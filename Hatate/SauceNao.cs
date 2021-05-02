﻿using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace Hatate
{
	class SauceNao
	{
		private List<Match> matches = new List<Match>();
		private bool dailyLimitExceeded = false;

		/*
		============================================
		Public
		============================================
		*/

		#region Public

		public async Task SearchFile(string filePath)
		{
			string response = await this.SearchImage(filePath);

			this.ParseResponseHtml(response);
		}

		#endregion Public

		/*
		============================================
		Private
		============================================
		*/

		#region Private

		/// <summary>
		/// Search an image on SauceNAO and return the HTML response.
		/// </summary>
		/// <param name="filePath"></param>
		private async Task<string> SearchImage(string filePath)
		{
			FileStream fs = null;

			try {
				fs = new FileStream(filePath, FileMode.Open);
			} catch (IOException) {
				return null; // May happen if the file is in use
			}

			MultipartFormDataContent form = new MultipartFormDataContent();
			form.Add(new StreamContent(fs), "file", "image.jpg");

			HttpClient httpClient = new HttpClient(new HttpClientHandler());
			HttpResponseMessage response = await httpClient.PostAsync("https://saucenao.com/search.php", form);

			fs.Close();
			fs.Dispose();

			return await response.Content.ReadAsStringAsync();
		}

		/// <summary>
		/// Parse HTML responded by SauceNAO.
		/// </summary>
		private void ParseResponseHtml(string html)
		{
			Supremes.Nodes.Document doc = null;

			try {
				doc = Supremes.Dcsoup.Parse(html, "utf-8");
			} catch {
				return;
			}

			Supremes.Nodes.Elements results = doc.Select("#middle > .result");

			if (results.Count < 1) {
				// Check if search limit was exceeded
				Supremes.Nodes.Element strong = doc.Select("strong").First;

				if (strong != null && strong.Text == "Daily Search Limit Exceeded.") {
					this.dailyLimitExceeded = true;
				}

				return;
			}

			foreach (Supremes.Nodes.Element result in results) {
				Supremes.Nodes.Elements sourceLinks = result.Select(".resultmiscinfo > a");
				Supremes.Nodes.Element resultimage = result.Select(".resultimage img").First;
				Supremes.Nodes.Element resultsimilarityinfo = result.Select(".resultsimilarityinfo").First;
				Supremes.Nodes.Element originalSourceLink = result.Select(".resultcontent .resultcontentcolumn a").First;

				// There were no booru links for this result but we have the link to pixiv/etc
				if (sourceLinks.Count == 0 && originalSourceLink != null) {
					Match match = new Match();

					match.Url = originalSourceLink.Attr("href");
					match.PreviewUrl = resultimage.Attr("src");
					match.Similarity = this.ParseSimilarity(resultsimilarityinfo.Text);
					match.DetermineSourceFromUrl();

					this.matches.Add(match);

					continue;
				}

				// We have booru links for this result
				foreach (Supremes.Nodes.Element sourceLink in sourceLinks) {
					Match match = new Match();

					match.Url = sourceLink.Attr("href");
					match.PreviewUrl = resultimage.Attr("src");
					match.Similarity = this.ParseSimilarity(resultsimilarityinfo.Text);
					match.DetermineSourceFromUrl();

					if (originalSourceLink != null) {
						match.SourceUrl = originalSourceLink.Attr("href");
					}

					this.matches.Add(match);
				}
			}
		}

		/// <summary>
		/// Parse similarity text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private float ParseSimilarity(string text)
		{
			text = text.Replace("%", "").Trim();

			CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			culture.NumberFormat.NumberDecimalSeparator = ".";

			try {
				return float.Parse(text, culture);
			} catch (System.Exception) {
				return 0;
			}
		}

		#endregion Private

		/*
		============================================
		Accessor
		============================================
		*/

		#region Accessor

		public ImmutableList<Match> Matches
		{
			get { return ImmutableList.Create(this.matches.ToArray()); }
		}

		public bool DailyLimitExceeded
		{
			get { return this.dailyLimitExceeded; }
		}

		#endregion Accessor
	}
}
