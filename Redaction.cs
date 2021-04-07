// ===========================================================================
//	©2013-2021 WebSupergoo. All rights reserved.
//
//	This source code is for use exclusively with the ABCpdf product under
//	the terms of the license for that product. Details can be found at
//
//		http://www.websupergoo.com/
//
//	This copyright notice must not be deleted and must be reproduced alongside
//	any sections of code extracted from this module.
// ===========================================================================

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;
using System.Diagnostics;

using WebSupergoo.ABCpdf12;
using WebSupergoo.ABCpdf12.Objects;
using WebSupergoo.ABCpdf12.Atoms;
using WebSupergoo.ABCpdf12.Operations;


namespace Redaction {
	// In this example, redactions operate on an atomic level of one text operator.
	// So if, for example, your text operator displays the text "the cat sat on the mat" 
	// and you redact the "cat" fragment, then this will also redact the other words too.
	// This kind of redaction is often appropriate when looking at areas of a document rather
	// than particular bits of text on a document.
	class SimpleRedaction {
		public static void RedactTextOps(Doc doc, IList<TextFragment> fragments) {
			if (fragments.Count == 0)
				return;
			ResourceTracker tracker = new ResourceTracker(doc);
			Dictionary<int, List<TextFragment>> streams = new Dictionary<int, List<TextFragment>>();
			foreach (TextFragment fragment in fragments) {
				List<TextFragment> redactions = null;
				int id = tracker.CopyOnWrite(fragment.StreamID, fragment.PageID);
				streams.TryGetValue(id, out redactions);
				if (redactions == null) {
					redactions = new List<TextFragment>();
					streams[id] = redactions;
				}
				redactions.Add(fragment);
			}
			foreach (KeyValuePair<int, List<TextFragment>> pair in streams) {
				// get data
				StreamObject stream = doc.ObjectSoup[pair.Key] as StreamObject;
				Debug.Assert(stream != null); // should never happen
				stream.Decompress(); // shouldn't really be necessary
				Debug.Assert(stream.Compressed == false); // should never happen
				byte[] data = stream.GetData();
				// construct new stream
				List<TextFragment> redactions = pair.Value;
				redactions.Sort((x, y) => x.StreamOffset.CompareTo(y.StreamOffset));
				MemoryStream mem = new MemoryStream(data.Length);
				int p = 0;
				foreach (TextFragment redaction in redactions) {
					if (p < redaction.StreamOffset) {
						mem.Write(data, p, redaction.StreamOffset - p);
						IDictionary<char, int> widths = redaction.Font.Widths;
						int defaultWidth = doc.GetInfoInt(redaction.Font.ID, "/DescendantFonts[0]*/DW*");
						if (defaultWidth == 0)
							defaultWidth = 1000;
						int width1000ths = 0;
						foreach (char c in redaction.Text) {
							int width = 0;
							if (!widths.TryGetValue(c, out width))
								width = defaultWidth;
							width1000ths += defaultWidth;
						}
						string tj = string.Format("[{0}] TJ\r\n", width1000ths);
						byte[] tjBytes = ASCIIEncoding.ASCII.GetBytes(tj);
						mem.Write(tjBytes, 0, tjBytes.Length);
					}
					p = redaction.StreamOffset + redaction.StreamLength;
				}
				mem.Write(data, p, data.Length - p);
				stream.SetData(mem.ToArray());
			}
		}
	}

	// In this example, redactions operate on an atomic level of one character. This
	// requires more complex code and there is a greater overhead but it is often more
	// appropriate when dealing with small bits of text.
	class FineRedaction {
		public static void RedactCharacters(Doc doc, IList<TextFragment> fragments) {
			if (fragments.Count == 0)
				return;
			ResourceTracker tracker = new ResourceTracker(doc);
			Dictionary<int, List<TextFragment>> streams = new Dictionary<int, List<TextFragment>>();
			foreach (TextFragment fragment in fragments) {
				List<TextFragment> redactions = null;
				int id = tracker.CopyOnWrite(fragment.StreamID, fragment.PageID);
				streams.TryGetValue(id, out redactions);
				if (redactions == null) {
					redactions = new List<TextFragment>();
					streams[id] = redactions;
				}
				redactions.Add(fragment);
			}
			foreach (KeyValuePair<int, List<TextFragment>> pair in streams) {
				// get data
				StreamObject stream = doc.ObjectSoup[pair.Key] as StreamObject;
				Debug.Assert(stream != null); // should never happen
				stream.Decompress(); // shouldn't really be necessary
				Debug.Assert(stream.Compressed == false); // should never happen
				byte[] data = stream.GetData();
				// construct new stream
				List<TextFragment> redactions = pair.Value;
				redactions.Sort((x, y) => x.StreamOffset.CompareTo(y.StreamOffset));
				MemoryStream mem = new MemoryStream(data.Length);
				int p = 0;
				for (int i = 0; i < redactions.Count; i++) {
					TextFragment redaction = redactions[i];
					RedactionGroup group = new RedactionGroup(data);
					for (int j = i; j < redactions.Count; j++) {
						if (!group.Add(redactions[j])) {
							i = j - 1;
							break;
						}
					}
					if (p < redaction.StreamOffset) {
						mem.Write(data, p, redaction.StreamOffset - p);
						byte[] tjBytes = group.GetTextOperator().GetData();
						mem.Write(tjBytes, 1, tjBytes.Length - 2);
					}
					p = redaction.StreamOffset + redaction.StreamLength;
				}
				mem.Write(data, p, data.Length - p);
				stream.SetData(mem.ToArray());
			}
		}

		private class RedactionGroup {
			private byte[] _data;
			private Dictionary<int, Tuple<string, bool[]>> _redactions;
			private TextFragment _text;

			public RedactionGroup(byte[] data) {
				_data = data;
				_redactions = new Dictionary<int, Tuple<string, bool[]>>();
			}

			public bool Add(TextFragment fragment) {
				if ((_text != null) && (_text.StreamOffset != fragment.StreamOffset))
					return false;
				_text = fragment;
				Tuple<string, bool[]> redaction = null;
				_redactions.TryGetValue(fragment.TextSpanIndex, out redaction);
				int pos = fragment.TextOffset;
				int len = fragment.Text.Length;
				Debug.Assert(fragment.RawText.Substring(pos).StartsWith(fragment.Text));
				if (redaction == null) {
					bool[] redacted = new bool[fragment.RawText.Length];
					for (int i = 0; i < len; i++)
						redacted[i + pos] = true;
					redaction = new Tuple<string, bool[]>(fragment.RawText, redacted);
					_redactions[fragment.TextSpanIndex] = redaction;
				}
				else {
					Debug.Assert(fragment.RawText == redaction.Item1);
					bool[] redacted = redaction.Item2;
					for (int i = 0; i < len; i++)
						redacted[i + pos] = true;
				}
				return true;
			}

			public ArrayAtom GetTextOperator() {
				Debug.Assert(_text != null);
				// get data and widths
				byte[] section = new byte[_text.StreamLength];
				Array.Copy(_data, _text.StreamOffset, section, 0, _text.StreamLength);
				IDictionary<char, int> widths = _text.Font.Widths;
				// convert to atoms
				ArrayAtom textOp = ArrayAtom.FromContentStream(section);
				string srcOp = ((OpAtom)textOp[1]).Text;
				int numParams = srcOp == "\"" ? 4 : 2;
				Debug.Assert(textOp.Count == numParams);
				ArrayAtom srcParams = textOp[numParams == 4 ? 2 : 0] as ArrayAtom;
				if (srcParams == null) {
					Debug.Assert(srcOp != "TJ");
					srcParams = new ArrayAtom();
					srcParams.Add(textOp[0]);
				}
				ArrayAtom dstParams = new ArrayAtom();
				// copy items
				int textSpanIndex = 0;
				for (int i = 0; i < srcParams.Count; i++) {
					Atom item = srcParams[i];
					if (item is StringAtom) {
						StringAtom str = (StringAtom)item;
						Tuple<string, bool[]> redaction = null;
						_redactions.TryGetValue(textSpanIndex++, out redaction);
						if (redaction == null) {
							dstParams.Add(item.Clone());
						}
						else {
							string encoded = str.Text;
							string whole = redaction.Item1;
							bool[] redacted = redaction.Item2;
							Debug.Assert((encoded.Length == whole.Length) || (encoded.Length == whole.Length * 2));
							int bytesPerChar = encoded.Length / whole.Length;
							int p1 = 0;
							while (p1 < redacted.Length) {
								bool hidden = redacted[p1];
								int width1000ths = 0;
								int p2;
								for (p2 = p1; p2 < redacted.Length; p2++) {
									if (hidden != redacted[p2])
										break;
									if (hidden)
										width1000ths += widths[whole[p2]];
								}
								if (hidden) {
									dstParams.Add(new NumAtom(-width1000ths));
								}
								else {
									string sub1 = encoded.Substring(p1 * bytesPerChar, (p2 - p1) * bytesPerChar);
									if (sub1.Length > 0)
										dstParams.Add(new StringAtom(sub1));
								}
								p1 = p2;
							}
						}
					}
					else if (item is NumAtom) {
						NumAtom num = (NumAtom)item;
						dstParams.Add(item.Clone());
					}
					else {
						Debug.Assert(false);
					}
				}
				// make new operators
				ArrayAtom newTextOp = new ArrayAtom();
				if (srcOp == "\'") {
					newTextOp.Add(new OpAtom("T*"));
				}
				else if (srcOp == "\"") {
					newTextOp.Add(textOp[0].Clone());
					newTextOp.Add(new OpAtom("Tw"));
					newTextOp.Add(textOp[1].Clone());
					newTextOp.Add(new OpAtom("Tc"));
					newTextOp.Add(new OpAtom("T*"));
				}
				newTextOp.Add(dstParams);
				newTextOp.Add(new OpAtom("TJ"));
				return newTextOp;
			}
		}
	}

	class FontTracker {
		public Doc _doc;
		private HashSet<int> _fonts;
		private HashSet<int> _containers;

		private FontTracker() { }

		public FontTracker(Doc doc) {
			_doc = doc;
			_fonts = new HashSet<int>();
			_containers = new HashSet<int>();
		}

		public void Keep(TextFragment fragment) {
			_fonts.Add(fragment.Font.ID);
			_containers.Add(fragment.PageID);
			_containers.Add(fragment.StreamID);
		}

		public void Purge() {
			_doc.PageNumber = 1;
			foreach (int id in _containers) {
				IndirectObject io = _doc.ObjectSoup[id];
				while (io != null) {
					Atom rez = io.Resolve(Atom.GetItem(io.Atom, "Resources"));
					DictAtom fonts = io.Resolve(Atom.GetItem(rez, "Font")) as DictAtom;
					if ((fonts != null) && (fonts.Count > 0)) {
						List<string> fontsToRemove = new List<string>();
						foreach (KeyValuePair<string, Atom> pair in fonts) {
							IndirectObject font = io.ResolveObj(pair.Value);
							if (font == null)
								continue; // shouldn't ever happen
							if (!_fonts.Contains(font.ID))
								fontsToRemove.Add(pair.Key);
						}
						foreach (string key in fontsToRemove)
							fonts.Remove(key);
					}
					io = io.ResolveObj(Atom.GetItem(io.Atom, "Parent"));
				}
			}
		}

		public void PurgeForm() {
			Catalog cat = _doc.ObjectSoup.Catalog;
			Atom form = cat.Resolve(Atom.GetItem(cat.Atom, "AcroForm"));
			if (form == null)
				return;
			// look at default appearance for documents and widgets
			HashSet<string> fontsToKeep = new HashSet<string>();
			Atom docDA = cat.Resolve(Atom.GetItem(form, "DA"));
			bool needsDocDA = false;
			for (int i = 0; i < _doc.PageNumber; i++) {
				_doc.PageNumber = i + 1;
				Page page = (Page)_doc.ObjectSoup[_doc.Page];
				foreach (Annotation annot in page.GetAnnotations()) {
					bool needsDA = false;
					if ((annot.FieldType == FieldType.Text) || (annot.FieldType == FieldType.Signature) || (annot.FieldType == FieldType.List) || (annot.FieldType == FieldType.Combo))
						needsDA = true;
					else {
						string subtype = Atom.GetName(cat.Resolve(Atom.GetItem(annot.Atom, "Subtype")));
						if ((subtype == "FreeText") || ((subtype == "Redact") && (Atom.GetItem(annot.Atom, "OverlayText") != null)))
							needsDA = true;
					}
					if (!needsDA)
						continue; // we can ignore this annotation
					Atom da = cat.Resolve(Atom.GetItem(annot.Atom, "DA"));
					if (da == null) {
						needsDocDA = true;
						continue; // we add in the doc DA at the end
					}
					KeepFonts(fontsToKeep, da);
				}
			}
			Debug.Assert((needsDocDA == false) || (docDA != null), "Both Widgets and Document fail to define required DA entry.");
			if (!needsDocDA) {
				Atom.RemoveItem(form, "DA");
				docDA = null;
			}
			if (docDA != null)
				KeepFonts(fontsToKeep, docDA);
			HashSet<string> fontsToRemove = new HashSet<string>();
			Atom rez = cat.Resolve(Atom.GetItem(form, "DR"));
			DictAtom fonts = cat.Resolve(Atom.GetItem(rez, "Font")) as DictAtom;
			if (fonts != null) {
				foreach (KeyValuePair<string, Atom> pair in fonts) {
					if (!fontsToKeep.Contains(pair.Key))
						fontsToRemove.Add(pair.Key);
				}
				foreach (string key in fontsToRemove)
					fonts.Remove(key);
			}
		}

		private static void KeepFonts(HashSet<string> fontsToKeep, Atom atomDA) {
			string da = Atom.GetText(atomDA);
			ArrayAtom array = ArrayAtom.FromContentStream(ASCIIEncoding.ASCII.GetBytes(da));
			var items = OpAtom.Find(array, new string[] { "Tf" });
			foreach (var pair in items) {
				Atom[] args = OpAtom.GetParameters(array, pair.Item2);
				Debug.Assert(args != null); // shouldn't really happen
				if (args != null)
					fontsToKeep.Add(Atom.GetName(args[0]));
			}
		}
	}

	class ImageRedaction {
		private Doc _doc;
		private ICollection<ImageProperties> _images;
		private bool _imagesCoverWholeDoc;
		private Dictionary<int, List<ImageRendition>> _redactions;
		private HashSet<ImageRendition> _redactionSet;

		private ImageRedaction() { }

		public ImageRedaction(Doc doc, ICollection<ImageProperties> images, bool wholeDoc) {
			_doc = doc;
			_images = images;
			_imagesCoverWholeDoc = wholeDoc;
			_redactions = new Dictionary<int, List<ImageRendition>>();
			_redactionSet = new HashSet<ImageRendition>();
		}

		public void MarkForRedaction(ImageRendition rendition) {
			List<ImageRendition> list = null;
			_redactions.TryGetValue(rendition.StreamID, out list);
			if (list == null) {
				list = new List<ImageRendition>();
				_redactions[rendition.StreamID] = list;
			}
			list.Add(rendition);
			_redactionSet.Add(rendition);
		}

		public void Redact() {
			Delete();
			Purge();
		}

		public void Delete() {
			foreach (KeyValuePair<int, List<ImageRendition>> pair in _redactions) {
				StreamObject so = (StreamObject)_doc.ObjectSoup[pair.Key];
				List<ImageRendition> renditions = pair.Value;
				renditions.Sort((i1, i2) => { return i1.StreamOffset.CompareTo(i2.StreamOffset); });
				if (!so.Decompress())
					throw new ArgumentException("Unable to decompress stream.");
				byte[] data = so.GetData();
				foreach (ImageRendition rendition in renditions) {
					for (int i = 0; i < rendition.StreamLength; i++)
						data[i + rendition.StreamOffset] = 0x20; // space
				}
				so.SetData(data);
				so.CompressFlate();
			}
		}

		public void Purge() {
			// First establish the count of images; per page (or xobject), per image.
			Dictionary<int, Dictionary<int, int>> imageCountPerParent = new Dictionary<int, Dictionary<int, int>>();
			foreach (ImageProperties image in _images) {
				foreach (ImageRendition rendition in image.Renditions) {
					int parent = rendition.StreamObject is FormXObject ? rendition.StreamID : rendition.PageID;
					Dictionary<int, int> imageCount = null;
					imageCountPerParent.TryGetValue(parent, out imageCount);
					if (imageCount == null) {
						imageCount = new Dictionary<int, int>();
						imageCountPerParent[parent] = imageCount;
					}
					int count = 0;
					if (!imageCount.TryGetValue(image.PixMap.ID, out count)) {
						imageCount[image.PixMap.ID] = 0;
						count = 0;
					}
					if (!_redactionSet.Contains(rendition))
						imageCount[image.PixMap.ID] = count + 1;
				}
			}
			// Then remove the ones that have been redacted
			foreach (KeyValuePair<int, Dictionary<int, int>> pair1 in imageCountPerParent) {
				HashSet<int> imagesToRemove = new HashSet<int>();
				foreach (KeyValuePair<int, int> pair2 in pair1.Value) {
					if (pair2.Value == 0) // no references left
						imagesToRemove.Add(pair2.Key);
				}
				if (imagesToRemove.Count > 0) {
					IndirectObject io = _doc.ObjectSoup[pair1.Key];
					while (io != null) {
						Atom rez = io.Resolve(Atom.GetItem(io.Atom, "Resources"));
						DictAtom xobjs = io.Resolve(Atom.GetItem(rez, "XObject")) as DictAtom;
						if ((xobjs != null) && (xobjs.Count > 0)) {
							List<string> namesToRemove = new List<string>();
							foreach (KeyValuePair<string, Atom> pair in xobjs) {
								IndirectObject xobj = io.ResolveObj(pair.Value);
								if (xobj == null)
									continue; // shouldn't ever happen
								if (imagesToRemove.Contains(xobj.ID))
									namesToRemove.Add(pair.Key);
							}
							foreach (string key in namesToRemove)
								xobjs.Remove(key);
						}
						io = _imagesCoverWholeDoc ? io.ResolveObj(Atom.GetItem(io.Atom, "Parent")) : null;
					}
				}
			}
		}
	}

	// It is possible, though uncommon, for page contents to be shared between pages. If this happens
	// and we redact part from one place in the document then it will also be redacted from other places
	// it appears.
	// This class keeps track of shared resources and performs a copy-on-write function as necessary.
	// You just pass it the stream you want to modify and the page it appears on and it passes you back
	// a stream that you can modify.
	// In most cases the stream that you pass in will be the same as the one you get back. However if the
	// stream is used on other pages, the function will duplicate the stream, adjust any references on the
	// page so that the new stream is used in place of the old, then pass back the duplicate.
	class ResourceTracker {
		private Doc _doc;
		private Dictionary<StreamObject, Dictionary<Page, int>> _pages;
		private Dictionary<StreamObject, Dictionary<Page, Tuple<DictAtom, string>>> _xobjs;
		private Dictionary<int, Dictionary<int, int>> _copyOnWriteCacheID;
		private bool _identity; // most of the time we don't need any copies

		private ResourceTracker() { }
		public ResourceTracker(Doc doc) {
			_doc = doc;
			_pages = new Dictionary<StreamObject, Dictionary<Page, int>>();
			_xobjs = new Dictionary<StreamObject, Dictionary<Page, Tuple<DictAtom, string>>>();
			_copyOnWriteCacheID = new Dictionary<int, Dictionary<int, int>>();
			// assemble all resources which may be shared
			foreach (Page page in doc.ObjectSoup.Catalog.Pages.GetPageArrayAll()) {
				StreamObject[] layers = page.GetLayers();
				for (int i = 0; i < layers.Length; i++) {
					StreamObject so = layers[i];
					Dictionary<Page, int> entry = null;
					_pages.TryGetValue(so, out entry);
					if (entry == null) {
						entry = new Dictionary<Page, int>();
						_pages[so] = entry;
					}
					Debug.Assert(!entry.ContainsKey(page));
					entry[page] = i;
				}
				IList<DictAtom> resourceDicts = page.GetResourceDictsByType("XObject", true, true, true, true, new HashSet<int>());
				foreach (DictAtom resources in resourceDicts) {
					foreach (var resource in resources) {
						FormXObject formxobj = page.ResolveObj(resource.Value) as FormXObject;
						if (formxobj == null)
							continue;
						Dictionary<Page, Tuple<DictAtom, string>> entry = null;
						_xobjs.TryGetValue(formxobj, out entry);
						if (entry == null) {
							entry = new Dictionary<Page, Tuple<DictAtom, string>>();
							_xobjs[formxobj] = entry;
						}
						entry[page] = new Tuple<DictAtom, string>(resources, resource.Key);
					}
				}
			}
			// remove unique entries just to simplify things
			List<StreamObject> unique = new List<StreamObject>();
			foreach (var pair in _pages)
				if (pair.Value.Count <= 1)
					unique.Add(pair.Key);
			foreach (var item in unique)
				_pages.Remove(item);
			unique.Clear();
			foreach (var pair in _xobjs)
				if (pair.Value.Count <= 1)
					unique.Add(pair.Key);
			foreach (var item in unique)
				_xobjs.Remove(item);
			_identity = (_pages.Count == 0) && (_xobjs.Count == 0);
		}

		public int CopyOnWrite(int streamID, int pageID) {
			if (_identity)
				return streamID;
			Dictionary<int, int> entry = null;
			_copyOnWriteCacheID.TryGetValue(pageID, out entry);
			if (entry != null) {
				int id = 0;
				entry.TryGetValue(streamID, out id);
				if (id > 0)
					return id;
			}
			else {
				entry = new Dictionary<int, int>();
				_copyOnWriteCacheID[pageID] = entry;
			}
			StreamObject so = UncachedCopyOnWrite(_doc.ObjectSoup[streamID] as StreamObject, _doc.ObjectSoup[pageID] as Page);
			Debug.Assert(so != null);
			entry[streamID] = so.ID;
			return so.ID;
		}

		private StreamObject UncachedCopyOnWrite(StreamObject so, Page page) {
			// this function must always be called from a function which caches the results
			if (_identity)
				return so;
			Dictionary<Page, int> entry1 = null;
			_pages.TryGetValue(so, out entry1);
			if (entry1 != null) {
				int index = entry1[page];
				StreamObject copy = (StreamObject)so.Clone();
				_doc.ObjectSoup.Add(copy);
				ArrayAtom array = page.Resolve(Atom.GetItem(page.Atom, "Contents")) as ArrayAtom;
				if (array != null) {
					Debug.Assert(array.Count > index);
					array[index] = new RefAtom(copy);
				}
				else {
					Debug.Assert(index == 0);
					Atom.SetItem(page.Atom, "Contents", new RefAtom(copy));
				}
				entry1.Remove(page);
				if (entry1.Count <= 1)
					_pages.Remove(so);
				return copy;
			}
			Dictionary<Page, Tuple<DictAtom, string>> entry2 = null;
			_xobjs.TryGetValue(so, out entry2);
			if (entry2 != null) {
				Tuple<DictAtom, string> index = entry2[page];
				Debug.Assert(so is FormXObject);
				StreamObject copy = (StreamObject)so.Clone();
				_doc.ObjectSoup.Add(copy);
				index.Item1[index.Item2] = new RefAtom(copy);
				entry2.Remove(page);
				return copy;
			}
			return so;
		}
	}
}
