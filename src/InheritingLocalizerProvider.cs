using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Localization;

namespace triaxis.AspNetCore.Localization.Inheritance
{
    /// <summary>
    /// Provides IStringLocalizer implementations that search for resources
    /// in the class hierarchy of the requesting type
    /// </summary>
    public class InheritingLocalizerProvider : IStringLocalizer
    {
        InheritingLocalizerProvider parent;
        IStringLocalizerFactory factory;
        Type type;
        IStringLocalizer localizer;
        IStringLocalizer shared;
        CultureInfo culture;

        /// <summary>
        /// Creates a new <see cref="InheritingLocalizerProvider" /> instance.
        /// </summary>
        /// <params>
        /// <param name="type">Type of the object for which localization is requested.</param>
        /// <param name="factory">
        /// An <see cref="IStringLocalizerFactory" /> providing actual <see cref="IStringLocalizer" /> instances.
        /// </param>
        /// <param name="culture">Optional forced <see cref="CultureInfo" /> of the localized strings.</param>
        /// </params>
        public InheritingLocalizerProvider(Type type, IStringLocalizerFactory factory, CultureInfo culture = null)
        {
            this.factory = factory;
            this.type = type;
            this.culture = culture;
            this.localizer = factory.Create(type);
            if (culture != null)
            {
                this.localizer = localizer.WithCulture(culture);
            }
        }

        /// <summary>
        /// Gets an <see cref="InheritingLocalizerProvider" /> for the <see cref="Type.BaseType" /> of the currently localized type.
        /// </summary>
        /// <returns>
        /// An <see cref="InheritingLocalizerProvider" /> for the <see cref="Type.BaseType" />
        /// of the currently localized type, or <c>null</c> when the end of the hierarchy is reached
        /// </returns>
        public InheritingLocalizerProvider Parent =>
            parent ?? (parent = CreateParent());

        /// <summary>
        /// Creates an <see cref="InheritingLocalizerProvider" /> for the
        /// <see cref="Type.BaseType" /> of the current localizer, as long as it makes sense.
        /// Localization stops at <see cref="System.Object" /> in the hierarchy.
        /// </summary>
        InheritingLocalizerProvider CreateParent()
        {
            var baseType = type.BaseType;
            if (baseType == null || baseType == typeof(object))
                return null;

            return new InheritingLocalizerProvider(baseType, factory, culture);
        }

        /// <summary>
        /// Gets the shared localizer for assembly of the localized type.
        /// </summary>
        /// <returns>
        /// An <see cref="IStringLocalizer" /> for shared resources defined
        /// in the assembly of the localized type.
        /// </returns>
        /// <remark>
        /// Shared resources are defined in a <c>_Shared.restext</c> or 
        /// <c>_Shared.resx</c> file in the
        /// <see cref="Microsoft.Extensions.Localization.LocalizationOptions.ResourcesPath" />.
        /// </remark>
        public IStringLocalizer Shared =>
            shared ?? (shared = factory.Create("_Shared", type.Assembly.FullName));

        /// <summary>
        /// Changes <see cref="LocalizedString" /> instances with <see cref="LocalizedString.ResourceNotFound" /> set to <c>true</c>
        /// to <c>null</c>s.
        /// </summary>
        static LocalizedString DiscardNotFound(LocalizedString str) => str == null || str.ResourceNotFound ? null : str;

        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        /// <params>
        /// <param name="name">The name of the string resource.</param>
        /// </params>
        /// <returns>The string resource as a <see cref="LocalizedString" />.</returns>
        public LocalizedString this[string name]
        {
            get
            {
                var res = localizer[name];
                if (res.ResourceNotFound)
                {
                    res = DiscardNotFound(Parent?[name]) ??
                        DiscardNotFound(Shared?[name]) ??
                        res;
                }
                return res;
            }
        }

        /// <summary>
        /// Gets the string resource with the given name and formatted with the
        /// supplied arguments.
        /// </summary>
        /// <params>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// </params>
        /// <returns>The formatted string resource as a <see cref="LocalizedString" />.</returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var res = localizer[name, arguments];
                if (res.ResourceNotFound)
                {
                    res = DiscardNotFound(Parent?[name, arguments]) ??
                        DiscardNotFound(Shared?[name, arguments]) ??
                        res;
                }
                return res;
            }
        }

        /// <summary>
        /// Gets all the string resources defined in the hierarchy of the original type.
        /// </summary>
        /// <params>
        /// <param name="includeParentCultures">
        /// A <see cref="Boolean" /> indicating whether to include strings from parent cultures.
        /// </param>
        /// </params>
        /// <returns>
        /// All the strings defined in the hierarchy.
        /// For strings defined on multiple levels in the hierarchy, only the
        /// most specific instance is returned.
        /// Shared strings are not returned.
        /// </returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var names = new HashSet<string>();
            for (var inst = this; inst != null; inst = inst.Parent)
            {
                foreach (var str in inst.localizer.GetAllStrings(includeParentCultures))
                {
                    if (names.Add(str.Name))
                        yield return str;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="IStringLocalizer" /> for a specific <see cref="CultureInfo" />.
        /// </summary>
        /// <params>
        /// <param name="culture">The <see cref="CultureInfo" /> to use.</param>
        /// </params>
        /// <returns>
        /// A culture-specific <see cref="IStringLocalizer" />.
        /// </returns>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new InheritingLocalizerProvider(type, factory, culture);
        }
    }
}
