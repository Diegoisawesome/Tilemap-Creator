﻿namespace TMC
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tilesetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tilemapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cTilesetWidth = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tTilesetHeight = new TMC.NumericToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tTilemapWidth = new TMC.NumericToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tTilemapHeight = new TMC.NumericToolStripTextBox();
            this.bResizeTilemap = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pTileset = new TMC.InterpolatedPictureBox();
            this.pTilemap = new TMC.InterpolatedPictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pTileset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pTilemap)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tilesetToolStripMenuItem,
            this.tilemapToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(676, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tilesetToolStripMenuItem
            // 
            this.tilesetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.createToolStripMenuItem});
            this.tilesetToolStripMenuItem.Name = "tilesetToolStripMenuItem";
            this.tilesetToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.tilesetToolStripMenuItem.Text = "Tileset";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(127, 26);
            this.createToolStripMenuItem.Text = "Create";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // tilemapToolStripMenuItem
            // 
            this.tilemapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem1});
            this.tilemapToolStripMenuItem.Name = "tilemapToolStripMenuItem";
            this.tilemapToolStripMenuItem.Size = new System.Drawing.Size(75, 24);
            this.tilemapToolStripMenuItem.Text = "Tilemap";
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(120, 26);
            this.openToolStripMenuItem1.Text = "Open";
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(120, 26);
            this.saveToolStripMenuItem1.Text = "Save";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cTilesetWidth,
            this.toolStripLabel2,
            this.tTilesetHeight,
            this.toolStripSeparator2,
            this.tTilemapWidth,
            this.toolStripLabel1,
            this.tTilemapHeight,
            this.bResizeTilemap});
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(676, 28);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // cTilesetWidth
            // 
            this.cTilesetWidth.Name = "cTilesetWidth";
            this.cTilesetWidth.Size = new System.Drawing.Size(121, 28);
            this.cTilesetWidth.Text = "0";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(16, 25);
            this.toolStripLabel2.Text = "x";
            // 
            // tTilesetHeight
            // 
            this.tTilesetHeight.Name = "tTilesetHeight";
            this.tTilesetHeight.ReadOnly = true;
            this.tTilesetHeight.Size = new System.Drawing.Size(100, 28);
            this.tTilesetHeight.Text = "0";
            this.tTilesetHeight.Value = 0;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // tTilemapWidth
            // 
            this.tTilemapWidth.Name = "tTilemapWidth";
            this.tTilemapWidth.Size = new System.Drawing.Size(100, 28);
            this.tTilemapWidth.Text = "0";
            this.tTilemapWidth.Value = 0;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(16, 25);
            this.toolStripLabel1.Text = "x";
            // 
            // tTilemapHeight
            // 
            this.tTilemapHeight.Name = "tTilemapHeight";
            this.tTilemapHeight.Size = new System.Drawing.Size(100, 28);
            this.tTilemapHeight.Text = "0";
            this.tTilemapHeight.Value = 0;
            // 
            // bResizeTilemap
            // 
            this.bResizeTilemap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bResizeTilemap.Image = ((System.Drawing.Image)(resources.GetObject("bResizeTilemap.Image")));
            this.bResizeTilemap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bResizeTilemap.Name = "bResizeTilemap";
            this.bResizeTilemap.Size = new System.Drawing.Size(24, 25);
            this.bResizeTilemap.Text = "Resize Tilemap";
            this.bResizeTilemap.Click += new System.EventHandler(this.bResizeTilemap_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Location = new System.Drawing.Point(0, 578);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(676, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 56);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.pTileset);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.pTilemap);
            this.splitContainer1.Size = new System.Drawing.Size(676, 522);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.TabIndex = 3;
            // 
            // pTileset
            // 
            this.pTileset.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pTileset.Location = new System.Drawing.Point(0, 0);
            this.pTileset.Name = "pTileset";
            this.pTileset.Size = new System.Drawing.Size(16, 16);
            this.pTileset.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pTileset.TabIndex = 0;
            this.pTileset.TabStop = false;
            this.pTileset.Paint += new System.Windows.Forms.PaintEventHandler(this.pTileset_Paint);
            // 
            // pTilemap
            // 
            this.pTilemap.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pTilemap.Location = new System.Drawing.Point(0, 0);
            this.pTilemap.Name = "pTilemap";
            this.pTilemap.Size = new System.Drawing.Size(16, 16);
            this.pTilemap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pTilemap.TabIndex = 0;
            this.pTilemap.TabStop = false;
            this.pTilemap.Paint += new System.Windows.Forms.PaintEventHandler(this.pTilemap_Paint);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 600);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Tilemap Creator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pTileset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pTilemap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tilesetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tilemapToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private InterpolatedPictureBox pTileset;
        private InterpolatedPictureBox pTilemap;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private NumericToolStripTextBox tTilemapWidth;
        private NumericToolStripTextBox tTilemapHeight;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripButton bResizeTilemap;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private NumericToolStripTextBox tTilesetHeight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox cTilesetWidth;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

