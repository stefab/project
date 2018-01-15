using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EtsyRobot.Storage.Framework.Utils
{
	static public class Extensions
	{
		/// <summary>
		/// Check whether there is an attribute of type <typeparamref name="T"/> specified on a type <paramref name="type"/>.
		/// </summary>
		/// <typeparam name="T">Type of an attribute to check.</typeparam>
		/// <param name="type">Type on which to check the attribute.</param>
		/// <returns><c>true</c>, if there is an attribute or <c>false</c> otherwise.</returns>
		/// <remarks>
		/// Method does not account for inherited attributes, only attributes specified directly on the <paramref name="type"/> are accounted.
		/// </remarks>
		static public bool HasAttribute<T>(this Type type) where T : Attribute
		{
			Guard.AgainstNull(type, "type");
			return Attribute.IsDefined(type, typeof(T), false);
		}

		/// <summary>
		/// Gets an attribute of type <typeparamref name="T"/> specified on a type <paramref name="type"/>.
		/// </summary>
		/// <typeparam name="T">Type of an attribute to get.</typeparam>
		/// <param name="type">Type on which to check the attribute.</param>
		/// <param name="inherit">The argument will be passed to <see cref="MemberInfo.GetCustomAttributes(System.Type,bool)"/>.</param>
		/// <returns>An instance of attribute or <c>null</c>.</returns>
		/// <exception cref="InvalidOperationException">There is more than one instance of the attribute on the type.</exception>
		/// <remarks>
		/// If there is more than one instance of an attribute, method will throw <see cref="InvalidOperationException"/>.
		/// Method does not account for inherited attributes, only attributes specified directly on the <paramref name="type"/> are accounted.
		/// </remarks>
		static public T GetSingleAttribute<T>(this Type type, bool inherit = false) where T : Attribute
		{
			Guard.AgainstNull(type, "type");
			object[] attrs = type.GetCustomAttributes(typeof(T), inherit);
			if (attrs.Length > 1)
			{
				throw new InvalidOperationException(
					string.Format("{0} attribute should be used zero or once per class.", typeof(T).Name));
			}
			return attrs.Length == 1 ? (T) attrs[0] : null;
		}

		/// <summary>
		/// Gets an attribute of type <typeparamref name="T"/> specified on a member <paramref name="member"/>.
		/// </summary>
		/// <typeparam name="T">Type of an attribute to get.</typeparam>
		/// <param name="member">Type member on which to check the attribute.</param>
		/// <returns>An instance of attribute or <c>null</c>.</returns>
		/// <exception cref="InvalidOperationException">There is more than one instance of the attribute on the member.</exception>
		/// <remarks>
		/// If there is more than one instance of an attribute, method will throw <see cref="InvalidOperationException"/>.
		/// Method does not account for inherited attributes, only attributes specified directly on the <paramref name="member"/> are accounted.
		/// </remarks>
		static public T GetSingleAttribute<T>(this MemberInfo member) where T : Attribute
		{
			Guard.AgainstNull(member, "member");
			object[] attrs = member.GetCustomAttributes(typeof(T), false);
			if (attrs.Length > 1)
			{
				throw new InvalidOperationException(
					string.Format("{0} attribute should be used zero or once per member.", typeof(T).Name));
			}
			return attrs.Length == 1 ? (T) attrs[0] : null;
		}

		/// <summary>
		/// Returns property name which is represented by given lambda expression.
		/// </summary>
		/// <param name="expression">Lambda expression.</param>
		/// <returns>Property name.</returns>
		static public string GetPropertyName(this LambdaExpression expression)
		{
			//Guard.Against(expression.Body is MemberExpression).With<ArgumentException>("expression must be MemberExpression.");

			string body = expression.Body.ToString();
			return body.Substring(body.IndexOf('.') + 1);
		}

		static public string GetPropertyName<TProperty>(this Expression<Func<TProperty>> property)
		{
			var lambda = (LambdaExpression) property;
			MemberExpression memberExpression;

			if (lambda.Body is UnaryExpression)
			{
				var unaryExpression = (UnaryExpression) lambda.Body;
				memberExpression = (MemberExpression) unaryExpression.Operand;
			}
			else
			{
				memberExpression = (MemberExpression) lambda.Body;
			}

			return memberExpression.Member.Name;
		}

		/// <summary>
		/// Returns property by path.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="propertyPath">Property path</param>
		/// <returns>Property name.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj"),
		 SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath")]
		// TODO: this method is not designed well - if using several levels and than try to get value from real object, it will fail
		// TODO: because after each dot the value of property shall be got.
		// TODO: also this method highly recommended to cover with unit-tests.
		// TODO: it's usage currently limited to some custom binding scenarios in Silverlight (AutocompleteCombobox).
		static public PropertyInfo GetPropertyByPath(object obj, string propertyPath)
		{
			Tuple<object, PropertyInfo> result = GetObjectPropertyByPath(obj, propertyPath);
			return result == null ? null : result.Item2;
		}

		/// <summary>
		/// Returns property value by path.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="propertyPath">Property path</param>
		/// <returns>Property name.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj"),
		 SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath")]
		static public object GetValueByPath(object obj, string propertyPath)
		{
			Tuple<object, PropertyInfo> result = GetObjectPropertyByPath(obj, propertyPath);
			return result == null ? null : result.Item1;
		}

		/// <summary>
		/// Returns methods of type and interfaces which it implements.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		static public IEnumerable<MethodInfo> GetMethodsOfTypeAndInterfaces(this Type type)
		{
			IEnumerable<Type> types = Enumerable.Repeat(type, 1).Concat(type.GetInterfaces());
			return types.SelectMany(t => t.GetMethods());
		}

		/// <summary>
		/// Returns ancestor chain, from given type to the root <see cref="Object"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		static public IEnumerable<Type> GetAncestors(this Type type)
		{
			for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
			{
				yield return currentType;
			}
		}

		static public bool IsNullable(this Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		static public Type UnwrapNullableType(this Type type)
		{
			return type.IsNullable() ? type.GetGenericArguments()[0] : type;
		}

		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj"),
		 SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath")]
		static private Tuple<object, PropertyInfo> GetObjectPropertyByPath(object obj, string propertyPath)
		{
			Guard.AgainstNullOrEmpty(propertyPath, "propertyPath");

			if (obj == null)
			{
				return null;
			}

			Type type = obj.GetType();
			object result = obj;

			PropertyInfo propertyInfo = null;
			foreach (string part in propertyPath.Split(new[] {'.'}))
			{
				if (propertyInfo != null)
				{
					type = propertyInfo.PropertyType;
				}

				propertyInfo = type.GetProperty(part);
				if (propertyInfo == null || propertyInfo.GetIndexParameters().Length > 0)
				{
					// Property not found OR property is indexed
					return null;
				}

				result = propertyInfo.GetValue(result, null);
				if (result == null)
				{
					break;
				}
			}

			return new Tuple<object, PropertyInfo>(result, propertyInfo);
		}
	}
}