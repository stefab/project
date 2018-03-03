using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace EtsyRobot.Storage.Framework.Utils
{
	/// <summary>
	///     Helper class for guard statements, that allow prettier code for guard clauses
	/// </summary>
	/// <example>
	///     Sample usage:
	///     <code>
	/// <![CDATA[
	/// Guard.Against(name.Length == 0).With<ArgumentException>("Name must have at least 1 char length");
	/// Guard.AgainstNull(obj, "obj");
	/// Guard.AgainstNullOrEmpty(name, "name", "Name must have a value");
	/// ]]>
	/// </code>
	/// </example>
	static public class Guard
	{
		/// <summary>
		///     Checks the value of the supplied <paramref name="value" /> and throws an
		///     <see cref="System.ArgumentNullException" /> if it is <see langword="null" />.
		/// </summary>
		/// <param name="value">The object to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <exception cref="System.ArgumentNullException">
		///     If the supplied <paramref name="value" /> is <see langword="null" />.
		/// </exception>
		static public void AgainstNull<T>(T value, string variableName) where T : class
		{
			AgainstNull(
				value, variableName,
				string.Format(CultureInfo.InvariantCulture, "'{0}' cannot be null.", variableName));
		}

		/// <summary>
		///     Checks the value of the supplied <paramref name="value" /> and throws an
		///     <see cref="System.ArgumentNullException" /> if it is <see langword="null" />.
		/// </summary>
		/// <param name="value">The object to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <param name="message">The message to include in exception description</param>
		/// <exception cref="System.ArgumentNullException">
		///     If the supplied <paramref name="value" /> is <see langword="null" />.
		/// </exception>
		static public void AgainstNull<T>(T value, string variableName, string message) where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(variableName, message);
			}
		}

		/// <summary>
		///     Checks the value of the supplied guid <paramref name="value" /> and throws an
		///     <see cref="System.ArgumentException" /> if it is <see langword="null" /> or is an empty guid.
		/// </summary>
		/// <param name="value">The guid value to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <exception cref="System.ArgumentException">
		///     If the supplied <paramref name="value" /> is <see langword="null" /> or contains an empty guid.
		/// </exception>
		static public void AgainstNullOrEmpty(Guid? value, string variableName)
		{
			if (value == null || value == Guid.Empty)
			{
				throw new ArgumentException(
					string.Format(CultureInfo.InvariantCulture, "'{0}' cannot be null or an empty guid : '{1}'.", variableName, value),
					variableName);
			}
		}

		/// <summary>
		///     Checks the value of the supplied string <paramref name="value" /> and throws an
		///     <see cref="System.ArgumentException" /> if it is <see langword="null" /> or empty.
		/// </summary>
		/// <param name="value">The string value to check.</param>
		/// <param name="variableName">The argument name.</param>
		/// <param name="options">The value trimming options.  Default is <see cref="TrimOption.DoTrim" />.</param>
		/// <exception cref="System.ArgumentException">
		///     If the supplied <paramref name="value" /> is <see langword="null" /> or empty.
		/// </exception>
		static public void AgainstNullOrEmpty(string value, string variableName, TrimOption options = TrimOption.DoTrim)
		{
			if (options == TrimOption.DoTrim && string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
			{
				string message = string.Format(
					CultureInfo.InvariantCulture,
					"'{0}' cannot be null or resolve to an empty string : '{1}'.", variableName, value);
				throw new ArgumentException(message, variableName);
			}
		}

		/// <summary>
		///     Checks the value of the supplied string <paramref name="value" /> and throws an
		///     <see cref="System.ArgumentException" /> if it is <see langword="null" /> or empty.
		/// </summary>
		/// <param name="value">The string value to check.</param>
		/// <param name="variableName">The variable name.</param>
		/// <param name="message">The message to include in exception description</param>
		/// <param name="options">The value trimming options.  Default is <see cref="TrimOption.DoTrim" />.</param>
		/// <exception cref="System.ArgumentException">
		///     If the supplied <paramref name="value" /> is <see langword="null" /> or empty.
		/// </exception>
		static public void AgainstNullOrEmpty(
			string value, string variableName, string message,
			TrimOption options = TrimOption.DoTrim)
		{
			if (options == TrimOption.DoTrim && string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
			{
				throw new ArgumentException(message, variableName);
			}
		}

		/// <summary>
		///     Checks the supplied condition and act with exception if condition resolves to <c>true</c>.
		/// </summary>
		static public Act Against(bool assertion)
		{
			return new Act(assertion);
		}

		#region TrimOption enum
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public enum TrimOption
		{
			DoTrim = 0,
			NoTrim = 1
		}
		#endregion

		#region Nested type: Act
		/// <summary>
		///     Represents action taken when assertion is true
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public sealed class Act
		{
			internal Act(bool assertion)
			{
				this._assertion = assertion;
			}

			/// <summary>
			///     Will throw an exception of type <typeparamref name="TException" />
			///     with the specified message if the "Against" assertion is true
			/// </summary>
			/// <typeparam name="TException">Exception type</typeparam>
			/// <param name="message">Exception message</param>
			public void With<TException>(string message) where TException : Exception
			{
				if (this._assertion)
				{
					throw (TException) Activator.CreateInstance(typeof(TException), message);
				}
			}

			private readonly bool _assertion;
		}
		#endregion
	}
}