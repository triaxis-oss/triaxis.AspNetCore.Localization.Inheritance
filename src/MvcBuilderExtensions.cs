using triaxis.AspNetCore.Localization.Inheritance;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring MVC data annotations localization.
    /// </summary>
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds MVC data annotations localization with inheritance to the application.
        /// </summary>
        /// <params>
        /// <param name="builder">The <see cref="IMvcBuilder" />.</param>
        /// </params>
        /// <returns>The <see cref="IMvcBuilder" />.</returns>
        public static IMvcBuilder AddDataAnnotationsLocalizationWithInheritance(this IMvcBuilder builder)
        {
            return builder.AddDataAnnotationsLocalization(setup =>
            {
                setup.DataAnnotationLocalizerProvider = (type, factory) =>
                    new InheritingLocalizerProvider(type, factory);
            });
        }

        /// <summary>
        /// Adds MVC data annotations localization with inheritance to the application.
        /// </summary>
        /// <params>
        /// <param name="builder">The <see cref="IMvcCoreBuilder" />.</param>
        /// </params>
        /// <returns>The <see cref="IMvcCoreBuilder" />.</returns>
        public static IMvcCoreBuilder AddDataAnnotationsLocalizationWithInheritance(this IMvcCoreBuilder builder)
        {
            return builder.AddDataAnnotationsLocalization(setup =>
            {
                setup.DataAnnotationLocalizerProvider = (type, factory) =>
                    new InheritingLocalizerProvider(type, factory);
            });
        }
    }
}
