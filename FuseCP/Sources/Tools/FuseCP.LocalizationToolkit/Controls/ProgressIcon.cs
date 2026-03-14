// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace FuseCP.LocalizationToolkit
{
	/// <summary>
	/// Animated Icon.
	/// </summary>
	[ToolboxItem(true)]
	public class ProgressIcon : System.Windows.Forms.UserControl
	{
		private Thread thread = null;
		private volatile bool stopRequested;
		private int currentFrame = 0;
		private int delayInterval = 50;
		private int pause = 0;
		private int loopCount = 0;
		private int currentLoop = 0;
		private int firstFrame = 0;
		private int lastFrame = 13;
		private ImageList images;
		private IContainer components;

		/// <summary>Initializes a new instance of the <b>AnimatedIcon</b> class.
		/// </summary>
		public ProgressIcon()
		{
			CheckForIllegalCrossThreadCalls = false;
			InitializeComponent();

			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		#region Dispose
		/// <summary>Clean up any resources being used.</summary>
		/// <param name="disposing"><see langword="true"/> to release both managed 
		/// and unmanaged resources; <see langword="false"/> to release 
		/// only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();

				StopAnimation();
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary>Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressIcon));
			this.images = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// images
			// 
			this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
			this.images.TransparentColor = System.Drawing.Color.Transparent;
			this.images.Images.SetKeyName(0, "ProgressImage00.bmp");
			this.images.Images.SetKeyName(1, "ProgressImage01.bmp");
			this.images.Images.SetKeyName(2, "ProgressImage02.bmp");
			this.images.Images.SetKeyName(3, "ProgressImage03.bmp");
			this.images.Images.SetKeyName(4, "ProgressImage04.bmp");
			this.images.Images.SetKeyName(5, "ProgressImage05.bmp");
			this.images.Images.SetKeyName(6, "ProgressImage06.bmp");
			this.images.Images.SetKeyName(7, "ProgressImage07.bmp");
			this.images.Images.SetKeyName(8, "ProgressImage08.bmp");
			this.images.Images.SetKeyName(9, "ProgressImage09.bmp");
			this.images.Images.SetKeyName(10, "ProgressImage10.bmp");
			this.images.Images.SetKeyName(11, "ProgressImage11.bmp");
			this.images.Images.SetKeyName(12, "ProgressImage12.bmp");
			// 
			// ProgressIcon
			// 
			this.Name = "ProgressIcon";
			this.Size = new System.Drawing.Size(30, 30);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>Starts animation from the beginning.
		/// </summary>
		public void StartAnimation()
		{
			StopAnimation();
			CheckRange();			// Check the first and the last frames
			stopRequested = false;

			thread = new Thread( new ThreadStart( threadFunc ) );
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>Stops animation not changing current frame number.
		/// </summary>
		public void StopAnimation()
		{
			if( thread != null )
			{
				stopRequested = true;
				if( thread.IsAlive )
					thread.Join(500);
				thread = null;
			}
			currentLoop = 0;
		}

		/// <summary>Displays the specified frame.</summary>
		/// <param name="frame">An index of the image stored in the <see cref="ImageList"/>.</param>
		public void ShowFrame2(int frame)
		{
			StopAnimation();

			if( frame >= 0 && frame < images.Images.Count )
				currentFrame = frame;
			else
				currentFrame = 0;

			Refresh();
		}

		/// <summary>Occurs when the control is redrawn.</summary>
		/// <param name="e">A <see cref="PaintEventArgs"/> that contains 
		/// the event data.</param>
		/// <remarks>The <b>OnPaint</b> method draws current image from 
		/// the <see cref="ImageList"/> if exists.</remarks>
		protected override void OnPaint(PaintEventArgs e)
		{
			// Draw a crossed rectangle if there is no frame to display

			if( images == null ||
				currentFrame < 0 || 
				currentFrame >= images.Images.Count )
			{
				if( this.Size.Width == 0 || this.Size.Height == 0 )
					return;

				Pen pen = new Pen( SystemColors.ControlText );
				e.Graphics.DrawRectangle( pen, 0, 0, this.Size.Width-1, this.Size.Height-1 );
				e.Graphics.DrawLine( pen, 0, 0, this.Size.Width, this.Size.Height );
				e.Graphics.DrawLine( pen, 0, this.Size.Height-1, this.Size.Width-1, 0 );
				pen.Dispose();
			}
			else
			{
				// Draw the current frame

				e.Graphics.DrawImage( images.Images[currentFrame], 0, 0, this.Size.Width, this.Size.Height );
			}
		}

		/// <summary>The method to be invoked when the thread begins executing.
		/// </summary>
		private void threadFunc()
		{
			bool wasPause = false;
			currentFrame = firstFrame;

			while( !stopRequested )
			{
				Refresh();						// Redraw the current frame
				wasPause = false;

				if( images != null )
				{
					currentFrame++;
					if( currentFrame > lastFrame ||
						currentFrame >= images.Images.Count )
					{
						if( pause > 0 )			// Sleep after every loop
						{
							Thread.Sleep( pause );
							wasPause = true;
						}

						currentFrame = firstFrame;
						if( loopCount != 0 )	// 0 is infinitive loop
						{
							currentLoop++;
						}
					}

					if( loopCount != 0 && currentLoop >= loopCount )
					{
						StopAnimation();		// The loop is completed
					}
				}
				if( !wasPause )					// That prevents summation (pause + delayInterval)
					Thread.Sleep( delayInterval );
			}
		}

		/// <summary>Check if the last frame is no less than the first one. 
		/// Otherwise, swap them.</summary>
		private void CheckRange()
		{
			if( lastFrame < firstFrame )
			{
				int tmp = firstFrame;
				firstFrame = lastFrame;
				lastFrame = tmp;
			}
		}
	}
}
