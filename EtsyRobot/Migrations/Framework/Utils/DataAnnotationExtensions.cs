using System;
using System.ComponentModel.DataAnnotations;

namespace EtsyRobot.Storage.Framework.Utils
{
	static public class DataAnnotationExtensions
	{
		static public DisplayAttribute GetDisplayAttribute(this Enum item)
		{
			Type enumType = item.GetType();
			object[] attributes = enumType.GetField(item.ToString()).GetCustomAttributes(typeof(DisplayAttribute), true);
			return attributes.Length > 0 ? (DisplayAttribute) attributes[0] : null;
		}

		static public string GetDisplayName(this Enum item, bool returnNameIfMissing = true)
		{
			DisplayAttribute attribute = GetDisplayAttribute(item);
			if (attribute != null)
			{
				return attribute.GetName();
			}
			return returnNameIfMissing ? item.ToString() : null;
		}
	}
}