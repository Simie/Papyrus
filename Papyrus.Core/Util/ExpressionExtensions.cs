/*
 * Copyright © 2014 Stompy Robot (http://www.stompyrobot.co.uk) (https://github.com/stompyrobot)
 * 
 * This program is licensed under the Microsoft Public License (Ms-PL). You should
 * have received a copy of the license along with the source code. If not, an online copy
 * of the license can be found at https://github.com/stompyrobot/Papyrus/wiki/License.
 */

using System.Linq.Expressions;
using System.Reflection;

namespace Papyrus.Core.Util
{

	/// <summary>
	/// Extension methods for handling expression trees
	/// </summary>
	public static class ExpressionExtensions
	{

		/// <summary>
		/// Converts an expression into a <see cref="MemberInfo"/>.
		/// </summary>
		/// <param name="expression">The expression to convert.</param>
		/// <returns>The member info.</returns>
		/// <remarks>Originates from Caliburn Micro, under MIT License. Copyright (c) 2010 Blue Spire Consulting, Inc.</remarks>
		public static MemberInfo GetMemberInfo(this Expression expression)
		{
			var lambda = (LambdaExpression)expression;

			MemberExpression memberExpression;
			if (lambda.Body is UnaryExpression) {
				var unaryExpression = (UnaryExpression)lambda.Body;
				memberExpression = (MemberExpression)unaryExpression.Operand;
			} else {
				memberExpression = (MemberExpression)lambda.Body;
			}

			return memberExpression.Member;
		}

	}

}
