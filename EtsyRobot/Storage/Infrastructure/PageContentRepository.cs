using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

using Newtonsoft.Json;

using EtsyRobot.Engine.PageModel;
using EtsyRobot.Properties;
using System.IO.Compression;

namespace EtsyRobot.Storage.Infrastructure
{
	public sealed class PageContentRepository
	{
		public PageContentRepository() : this(Settings.Default)
		{}

		internal PageContentRepository(Settings settings)
		{
			this._settings = settings;
		}

		public PageContent LoadReferenceContent(int id)
		{
			return this.Load(id, "reference");
		}

		public PageContent LoadTestContent(int id)
		{
			return this.Load(id, "test");
		}

		public void SaveReferenceContent(PageContent content, int id)
		{
			this.Save(content, id, "reference");
		}

		public void SaveTestContent(PageContent content, int id)
		{
			this.Save(content, id, "test");
		}

		public void DeleteReferenceContent(int id)
		{
			this.Delete(id, "reference");
		}

		public void DeleteTestContent(int id)
		{
			this.Delete(id, "test");
		}

		public string BuildReferenceScreenshotFileName(int id)
		{
			return this.BuildFileName(id, "reference", ".jpg");
		}

		public string BuildReferenceScreenshotFileNameOld(int id)
		{
			return this.BuildFileNameOld(id, "reference", ".jpg");
		}

		public string BuildTestScreenshotFileName(int id)
		{
			return this.BuildFileName(id, "test", ".jpg");
		}
		public string BuildTestScreenshotFileNameOld(int id)
		{
			return this.BuildFileNameOld(id, "test", ".jpg");
		}

		public IDisposable AcquireLock(int id)
		{
			string fileName = this.BuildLockName(id, "", ".lock");
			return FileMutex.Acquire(fileName, TimeSpan.FromSeconds(30));
		}

		private PageContent Load(int id, string suffix)
		{
			EnsureDirectoryExists(this._settings.StoragePath);

			string imageFileName = this.BuildFileName(id, suffix, ".jpg");
			Image screenshot = null; // = Image.FromFile(imageFileName);
            if (File.Exists(imageFileName)) {
                screenshot = Image.FromFile(imageFileName);
            }
            else
            {
                // find by old path
                imageFileName = this.BuildFileNameOld(id, suffix, ".jpg");
                // may throw exception FileNotFound
                screenshot = Image.FromFile(imageFileName);
            }

			string contentFileName = this.BuildFileName(id, suffix, ".json.gz");
			PageContent content;
            if (File.Exists(contentFileName)) {
                using (var reader = new FileStream(contentFileName, FileMode.Open, FileAccess.Read))
                using (var _gzstream = new System.IO.Compression.GZipStream(reader, CompressionMode.Decompress))
			    using (var jsonReader = new JsonTextReader(new StreamReader(_gzstream)))
			    {
				    content = new JsonSerializer().Deserialize<PageContent>(jsonReader);
			    }  
            }
            else {
                contentFileName = this.BuildFileNameOld(id, suffix, ".json");
			    using (var reader = new JsonTextReader(new StreamReader(contentFileName)))
			    {
				    content = new JsonSerializer().Deserialize<PageContent>(reader);
			    }            
            }
			content.Screenshot = screenshot;
			return content;
		}

		private void Save(PageContent content, int id, string suffix)
		{
			EnsureDirectoryExists(this._settings.StoragePath);

			string imageFileName = this.BuildFileName(id, suffix, ".jpg");
			if (!File.Exists(imageFileName))
			{
			    EnsureDirectoryExists(Path.GetDirectoryName(imageFileName));				
                content.Screenshot.Save(imageFileName, ImageFormat.Jpeg);
			}

			string contentFileName = this.BuildFileName(id, suffix, ".json.gz");
			if (!File.Exists(contentFileName))
			{
				// compress and  save
                //using (var writer = new StreamWriter(contentFileName))
				//{
				//	new JsonSerializer().Serialize(writer, content);
				//}
                EnsureDirectoryExists(Path.GetDirectoryName(contentFileName));
                using (var writer = new StreamWriter(contentFileName).BaseStream)
                using (var _gzstream = new System.IO.Compression.GZipStream(writer, CompressionLevel.Optimal))
                using (var streamWriter = new StreamWriter(_gzstream))
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    new JsonSerializer().Serialize(jsonWriter, content);
                }
			}
		}

		private void Delete(int id, string suffix)
		{
			string imageFileName = this.BuildFileName(id, suffix, ".jpg");
			if (File.Exists(imageFileName))
			{
				File.Delete(imageFileName);
			}
            else
            {
                // old storage case
                imageFileName = BuildFileNameOld(id, suffix, ".jpg");
                if (File.Exists(imageFileName)) {
                    File.Delete(imageFileName);
                }
            }

			string elementsFileName = this.BuildFileName(id, suffix, ".json.gz");
			if (File.Exists(elementsFileName))
			{
				File.Delete(elementsFileName);
			}
            else
            {
                //  old storage case
                elementsFileName = this.BuildFileName(id, suffix, ".json");
                if (File.Exists(elementsFileName)) {
                    File.Delete(elementsFileName);
                }
            }
		}

        private string BuildLockName(int id, string suffix, string extension)
        {
            string fileName = string.Concat(id.ToString(CultureInfo.InvariantCulture), "_", suffix, extension);
            return Path.Combine(this._settings.StoragePath, fileName);
        }

		private string BuildFileName(int id, string suffix, string extension)
		{
            int folderId = (int)(id / 1000);
            string subFolder = string.Format("{0:d}", folderId);
            string fileName = string.Concat(id.ToString(CultureInfo.InvariantCulture), "_", suffix, extension);
			return Path.Combine(this._settings.StoragePath, subFolder, fileName);
		}

		private string BuildFileNameOld(int id, string suffix, string extension)
		{
			string fileName = string.Concat(id.ToString(CultureInfo.InvariantCulture), "_", suffix, extension);
			return Path.Combine(this._settings.StoragePath, fileName);
		}


		static private void EnsureDirectoryExists(string path)
		{
            // Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		private readonly Settings _settings;
	}
}