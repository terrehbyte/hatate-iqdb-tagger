﻿using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Forms;

namespace Hatate
{
	/// <summary>
	/// Interaction logic for Compare.xaml
	/// </summary>
	public partial class Compare : Window
	{
		private Result result = null;
		private BitmapImage bitmapImage = null;

		public Compare()
		{
			InitializeComponent();

			this.Owner = App.Current.MainWindow;
			Screen screen = Screen.FromPoint(new System.Drawing.Point((int)this.Left, (int)this.Top));

			if (App.Current.MainWindow.Left + App.Current.MainWindow.Width + this.Width < screen.WorkingArea.Width) { // Right
				this.Left = App.Current.MainWindow.Left + App.Current.MainWindow.Width;
				this.Top = App.Current.MainWindow.Top;
			} else if (App.Current.MainWindow.Left - this.Width > 0) { // Left
				this.Left = App.Current.MainWindow.Left - this.Width;
				this.Top = App.Current.MainWindow.Top;
			} else if (App.Current.MainWindow.Top + App.Current.MainWindow.Height + this.Height < screen.WorkingArea.Height) { // Bottom
				this.Left = App.Current.MainWindow.Left;
				this.Top = App.Current.MainWindow.Top + App.Current.MainWindow.Height;
			} else if (App.Current.MainWindow.Top - this.Height > 0) { // Top
				this.Left = App.Current.MainWindow.Left;
				this.Top = App.Current.MainWindow.Top - this.Height;
			} else { // Center
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}
		}

		/*
		============================================
		Public
		============================================
		*/

		public void LoadResultImages(Result result)
		{
			// Already loaded
			if (this.result != null && result.ImagePath == this.result.ImagePath && result.Full == this.result.Full) {
				this.Status(result.Full);

				return;
			}

			this.result = result;
			this.Image_Local.Source = null;
			this.Image_Remote.Source = null;

			try {
				this.Image_Local.Source = App.CreateBitmapImage(result.ImagePath);
				this.Image_Remote.Source = new BitmapImage(new Uri(result.PreviewUrl));
			} catch (Exception) {
				this.Status("Image loading failed.");

				return;
			}

			if (result.Full != null) {
				this.LoadResultFullImage(result);
			} else {
				this.Status("Full image not available for this match, displaying preview image instead.");
			}

			this.Show();
		}

		/*
		============================================
		Private
		============================================
		*/

		private void Status(string message)
		{
			this.Label_Status.Content = message;
		}

		private void LoadResultFullImage(Result result)
		{
			this.bitmapImage = new BitmapImage();
			Uri fullImageUri = null;

			try {
				fullImageUri = new Uri(result.Full);
			} catch (UriFormatException) {
				this.Status("Invalid image URL: " + result.Full);

				return;
			}

			this.bitmapImage.BeginInit();
			this.bitmapImage.UriSource = fullImageUri;
			this.bitmapImage.CacheOption = BitmapCacheOption.OnDemand;
			this.bitmapImage.DownloadProgress += this.FullImageLoadingProgress;
			this.bitmapImage.DownloadCompleted += this.FullImageLoadingCompleted;
			this.bitmapImage.DownloadFailed += this.FullImageLoadingCompleted;
			this.bitmapImage.EndInit();

			// Image is already cached
			if (!this.bitmapImage.IsDownloading) {
				this.Image_Remote.Source = this.bitmapImage;
				this.Status(result.Full);
			}
		}

		/*
		============================================
		Event
		============================================
		*/

		private void FullImageLoadingProgress(object sender, DownloadProgressEventArgs e)
		{
			this.Status("Loading " + e.Progress + "% " + this.result.Full);
		}

		private void FullImageLoadingCompleted(object sender, EventArgs args)
		{
			BitmapImage bitmapImage = (BitmapImage)sender;
			bitmapImage.Freeze();

			this.Image_Remote.Source = bitmapImage;
			this.Status("Loaded.");
		}

		private void FullImageLoadingFailed(object sender, ExceptionEventArgs args)
		{
			this.Status("Higher quality image loading failed.");
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Escape) {
				this.Close();
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			this.bitmapImage = null;
		}
	}
}
