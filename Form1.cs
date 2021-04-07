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
using System.Reflection;

using WebSupergoo.ABCpdf12;
using WebSupergoo.ABCpdf12.Objects;
using WebSupergoo.ABCpdf12.Atoms;
using WebSupergoo.ABCpdf12.Operations;

namespace Redaction
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form {
		private System.Windows.Forms.Button button1;
		private Button button2;
		private Button button4;
		private Button button5;
		private PictureBox pictureBox1;
		private GroupBox groupBox1;
		private TextBox textBoxFind;
		private RadioButton radioButtonFine;
		private RadioButton radioButtonCoarse;
		private GroupBox groupBox2;
		private Label label1;
		private TextBox textBoxRect;
		private GroupBox groupBox3;
		private Label label2;
		private TextBox textBoxFont;
		private Button button3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxFind = new System.Windows.Forms.TextBox();
			this.radioButtonFine = new System.Windows.Forms.RadioButton();
			this.radioButtonCoarse = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxRect = new System.Windows.Forms.TextBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxFont = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(643, 336);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(202, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Redact Text Area";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Location = new System.Drawing.Point(643, 365);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(201, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "Redact Text Words";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button4
			// 
			this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button4.Location = new System.Drawing.Point(642, 394);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(201, 23);
			this.button4.TabIndex = 3;
			this.button4.Text = "Redact Text Fonts";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button5.Location = new System.Drawing.Point(641, 423);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(202, 23);
			this.button5.TabIndex = 4;
			this.button5.Text = "Redact Image Area";
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Location = new System.Drawing.Point(15, 13);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(620, 865);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.textBoxFind);
			this.groupBox1.Controls.Add(this.radioButtonFine);
			this.groupBox1.Controls.Add(this.radioButtonCoarse);
			this.groupBox1.Location = new System.Drawing.Point(641, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(201, 184);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Text Selection - Marked in Blue";
			// 
			// textBoxFind
			// 
			this.textBoxFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFind.Location = new System.Drawing.Point(16, 66);
			this.textBoxFind.Multiline = true;
			this.textBoxFind.Name = "textBoxFind";
			this.textBoxFind.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxFind.Size = new System.Drawing.Size(172, 101);
			this.textBoxFind.TabIndex = 12;
			this.textBoxFind.Text = "plant\r\nflower";
			this.textBoxFind.TextChanged += new System.EventHandler(this.textBoxFind_TextChanged);
			// 
			// radioButtonFine
			// 
			this.radioButtonFine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioButtonFine.AutoSize = true;
			this.radioButtonFine.Checked = true;
			this.radioButtonFine.Location = new System.Drawing.Point(15, 19);
			this.radioButtonFine.Name = "radioButtonFine";
			this.radioButtonFine.Size = new System.Drawing.Size(85, 17);
			this.radioButtonFine.TabIndex = 11;
			this.radioButtonFine.TabStop = true;
			this.radioButtonFine.Text = "Fine Grained";
			this.radioButtonFine.UseVisualStyleBackColor = true;
			// 
			// radioButtonCoarse
			// 
			this.radioButtonCoarse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioButtonCoarse.AutoSize = true;
			this.radioButtonCoarse.Location = new System.Drawing.Point(15, 43);
			this.radioButtonCoarse.Name = "radioButtonCoarse";
			this.radioButtonCoarse.Size = new System.Drawing.Size(98, 17);
			this.radioButtonCoarse.TabIndex = 10;
			this.radioButtonCoarse.Text = "Coarse Grained";
			this.radioButtonCoarse.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.textBoxRect);
			this.groupBox2.Location = new System.Drawing.Point(641, 202);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 61);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Area Selection - Marked in Green";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Rect";
			// 
			// textBoxRect
			// 
			this.textBoxRect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxRect.Location = new System.Drawing.Point(49, 22);
			this.textBoxRect.Name = "textBoxRect";
			this.textBoxRect.Size = new System.Drawing.Size(139, 20);
			this.textBoxRect.TabIndex = 15;
			this.textBoxRect.Text = "5 5 400 80";
			this.textBoxRect.TextChanged += new System.EventHandler(this.textBoxRect_TextChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.textBoxFont);
			this.groupBox3.Location = new System.Drawing.Point(643, 269);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(200, 61);
			this.groupBox3.TabIndex = 17;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Font Selection - Marked in Red";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "Name";
			// 
			// textBoxFont
			// 
			this.textBoxFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFont.Location = new System.Drawing.Point(49, 22);
			this.textBoxFont.Name = "textBoxFont";
			this.textBoxFont.Size = new System.Drawing.Size(139, 20);
			this.textBoxFont.TabIndex = 15;
			this.textBoxFont.Text = "Helvetica-Bold";
			this.textBoxFont.TextChanged += new System.EventHandler(this.textBoxFont_TextChanged);
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(641, 475);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(202, 23);
			this.button3.TabIndex = 18;
			this.button3.Text = "Reload Document";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(854, 890);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		string _src = "";

		private void Form1_Load(object sender, EventArgs e) {
			Reload();
		}

		private void textBoxRect_TextChanged(object sender, EventArgs e) {
			UpdateDoc();
		}

		private void textBoxFont_TextChanged(object sender, EventArgs e) {
			UpdateDoc();
		}

		private void textBoxFind_TextChanged(object sender, EventArgs e) {
			UpdateDoc();
		}

		private void Reload() {
			string theBase = Directory.GetCurrentDirectory();
			string theRez = Directory.GetParent(theBase).Parent.FullName + "\\";
			foreach (string file in Directory.GetFiles(theRez, "*.pdf")) {
				if (!Path.GetFileName(file).StartsWith("_")) {
					_src = file;
					UpdateDoc();
					break;
				}
			}
		}

		private void UpdateDoc() {
			using (Doc doc = LoadDoc()) {
			} // do nothing
		}

		private void button1_Click(object sender, System.EventArgs e) {
			// Here we look at an area on the page and redact the text in it
			XRect toRedact = new XRect(textBoxRect.Text);
			using (Doc doc = new Doc()) {
				doc.Read(_src);
				TextOperation op = new TextOperation(doc);
				op.PageContents.AddPages(1);
				string text = op.GetText(toRedact, 1);
				IList<TextFragment> fragments = op.Select(0, text.Length);
				if (radioButtonCoarse.Checked)
					SimpleRedaction.RedactTextOps(doc, fragments);
				else
					FineRedaction.RedactCharacters(doc, fragments);
				doc.Flatten();
				string dst = Path.Combine(Directory.GetParent(_src).FullName, "_" + Path.GetRandomFileName() + ".pdf");
				doc.Save(dst);
				_src = dst;
				UpdateDoc();
			}
		}

		private void button2_Click(object sender, EventArgs e) {
			// here we search for some text on the page and then redact it
			using (Doc doc = new Doc()) {
				doc.Read(_src);
				using (XRect rect = doc.Rect) {
					List<TextFragment> fragments = FindText(doc);
					if (radioButtonCoarse.Checked)
						SimpleRedaction.RedactTextOps(doc, fragments);
					else
						FineRedaction.RedactCharacters(doc, fragments);
					doc.Flatten();
				}
				string dst = Path.Combine(Directory.GetParent(_src).FullName, "_" + Path.GetRandomFileName() + ".pdf");
				doc.Save(dst);
				_src = dst;
				UpdateDoc();
			}
		}

		private void button3_Click(object sender, EventArgs e) {
			Reload();
		}

		private void button4_Click(object sender, EventArgs e) {
			// Here we look at specific style - fonts - that we don't want.
			// However the crucial difference between this and the other examples is
			// that we purge the unused fonts to remove all references from the document.
			using (Doc doc = new Doc()) {
				doc.Read(_src);
				FontTracker tracker = new FontTracker(doc);
				for (int i = 0; i < doc.PageNumber; i++) {
					doc.PageNumber = i + 1;
					using (XRect rect = doc.Rect) {
						List<TextFragment> toKeep = new List<TextFragment>();
						List<TextFragment> fragments = FindFont(doc, toKeep);
						foreach (TextFragment fragment in toKeep)
							tracker.Keep(fragment);		
						SimpleRedaction.RedactTextOps(doc, fragments);
						doc.Flatten();
					}
				}
				tracker.PurgeForm();
				tracker.Purge();
				string dst = Path.Combine(Directory.GetParent(_src).FullName, "_" + Path.GetRandomFileName() + ".pdf");
				doc.Save(dst);
				_src = dst;
				UpdateDoc();
			}
		}

		private void button5_Click(object sender, EventArgs e) {
			// here we find and remove images which overlap with our area of interest
			XRect toRedact = new XRect(textBoxRect.Text);
			using (Doc doc = new Doc()) {
				doc.Read(_src);
				ImageOperation op = new ImageOperation(doc);
				op.PageContents.AddPages(1);
				ICollection<ImageProperties> images = op.GetImageProperties();
				ImageRedaction redaction = new ImageRedaction(doc, images, doc.PageCount == 1);
				foreach (ImageProperties image in images)
					foreach (ImageRendition rendition in image.Renditions)
						if (rendition.BoundingBox.IntersectsWith(toRedact))
							redaction.MarkForRedaction(rendition);
				redaction.Redact();
				string dst = Path.Combine(Directory.GetParent(_src).FullName, "_" + Path.GetRandomFileName() + ".pdf");
				doc.Save(dst);
				_src = dst;
				UpdateDoc();
			}
		}

		private Doc LoadDoc() {
			Doc doc = null;
			Bitmap bitmap = null;
			try {
				if (File.Exists(_src)) {
					// NB It's a bit inefficient calling both FindText and FindFont
					// but it makes the code a good deal easier to understand.
					doc = new Doc();
					doc.Read(_src);
					doc.Color.SetRgb(0, 255, 0); // green
					// show where rect is
					doc.Rect.String = textBoxRect.Text;
					doc.FrameRect();
					// show where text is
					doc.Color.SetRgb(0, 0, 255); // blue
					doc.Width = 0; // hairlines
					foreach (TextFragment fragment in FindText(doc)) {
						fragment.Focus();
						doc.FrameRect(); // show where fragments have been found
					}
					// show where fonts are
					doc.Color.SetRgb(255, 0, 0); // red
					foreach (TextFragment fragment in FindFont(doc, null)) {
						fragment.Focus();
						doc.FrameRect(); // show where fragments have been found
					}
					// render
					doc.Rect.String = doc.CropBox.String;
					bitmap = doc.Rendering.GetBitmap();
				}
			}
			catch {
			}
			pictureBox1.Image = bitmap;
			return doc;
		}

		private List<TextFragment> FindText(Doc doc) {
			TextOperation op = new TextOperation(doc);
			op.PageContents.AddPages(1); // hardwired to page one for these examples
			string text = op.GetText();
			List<TextFragment> fragments = new List<TextFragment>();
			foreach (string candidate in textBoxFind.Lines) {
				if (candidate.Length == 0)
					continue;
				int pos = -1;
				while (true) {
					pos = text.IndexOf(candidate, pos + 1);
					if (pos < 0)
						break;
					fragments.AddRange(op.Select(pos, candidate.Length));
				}
			}
			return fragments;
		}

		private List<TextFragment> FindFont(Doc doc, List<TextFragment> others) {
			TextOperation op = new TextOperation(doc);
			op.PageContents.AddPages(1); // hardwired to page one for these examples
			string text = op.GetText();
			List<TextFragment> fragments = new List<TextFragment>();
			if (text.Length > 0) {
				IList<TextFragment> candidates = op.Select(0, text.Length);
				foreach (TextFragment candidate in candidates) {
					if (candidate.Font.BaseFont.Contains(textBoxFont.Text))
						fragments.Add(candidate);
					else if (others != null)
						others.Add(candidate);
				}
			}
			return fragments;
		}
	}
}
