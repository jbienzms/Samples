using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ThemeSample
{
    /// <summary>
    /// Manages theme resources for an app.
    /// </summary>
    static public class ThemeManager
    {
        #region Member Variables
        static private Dictionary<string, ResourceDictionary> colorResources = new Dictionary<string, ResourceDictionary>();
        static private ResourceDictionary currentColors;
        static private ResourceDictionary currentShapes;
        static private bool isInitialized;
        static private Dictionary<string, ResourceDictionary> shapeResources = new Dictionary<string, ResourceDictionary>();
        #endregion // Member Variables

        #region Internal Methods
        /// <summary>
        /// Loads a Xaml ResourceDictionary into memory from an embedded resource.
        /// </summary>
        /// <param name="name">
        /// The name of the xaml file without the extension.
        /// </param>
        /// <returns>
        /// The loaded ResourceDictionary.
        /// </returns>
        static private ResourceDictionary LoadResources(string name)
        {
            return new ResourceDictionary { Source = new Uri($"ms-appx:///AppThemes/{name}.xaml", UriKind.Absolute) };
        }

        /// <summary>
        /// Loads color resources and caches them.
        /// </summary>
        static private ResourceDictionary GetColorResources(string name)
        {
            if (!colorResources.ContainsKey(name))
            {
                colorResources[name] = LoadResources($"{name}Colors");
            }
            return colorResources[name];
        }

        /// <summary>
        /// Loads shape resources and caches them.
        /// </summary>
        static private ResourceDictionary GetShapeResources(string name)
        {
            if (!shapeResources.ContainsKey(name))
            {
                shapeResources[name] = LoadResources($"{name}Shapes");
            }
            return shapeResources[name];
        }
        #endregion // Internal Methods

        #region Public Methods
        /// <summary>
        /// Initializes the theme manager.
        /// </summary>
        static public void Initialize()
        {
            // Only do it once
            if (isInitialized) { return; }
            isInitialized = true;

            // Load from embedded resources
            currentColors = GetColorResources("Default");
            currentShapes = GetShapeResources("Default");

            // Remove the merged dictionaries (there to support design time) and change them to dynamic theme versions
            // App.Current.Resources.MergedDictionaries.Clear();
            App.Current.Resources.MergedDictionaries.Add(currentColors);
            App.Current.Resources.MergedDictionaries.Add(currentShapes);
        }

        /// <summary>
        /// Applies the named color theme.
        /// </summary>
        /// <param name="name">
        /// The name of the color theme to apply. This string should match the name 
        /// of a file in the AppThemes folder. For example "Red" would correspond to 
        /// AppThemes\RedColors.xaml.
        /// </param>
        /// <returns>
        /// <c>true</c> if the resources were applied; otherwise <c>false</c>. This method will return 
        /// <c>false</c> if the specified theme is already applied.
        /// </returns>
        static public bool UpdateColors(string name)
        {
            // Get new colors
            var newColors = GetColorResources(name);

            // Ensure changing
            if (currentColors == newColors) { return false; }

            // Update
            App.Current.Resources.MergedDictionaries.Remove(currentColors);
            App.Current.Resources.MergedDictionaries.Add(newColors);
            currentColors = newColors;

            // Changed
            return true;
        }

        /// <summary>
        /// Applies the named shape theme.
        /// </summary>
        /// <param name="name">
        /// The name of the shape theme to apply. This string should match the name 
        /// of a file in the AppThemes folder. For example "Rect" would correspond to 
        /// AppThemes\RectShapes.xaml.
        /// </param>
        /// <returns>
        /// <c>true</c> if the resources were applied; otherwise <c>false</c>. This method will return 
        /// <c>false</c> if the specified theme is already applied.
        /// </returns>
        static public bool UpdateShapes(string name)
        {
            // Get new Shapes
            var newShapes = GetShapeResources(name);

            // Ensure changing
            if (currentShapes == newShapes) { return false; }

            // Update
            App.Current.Resources.MergedDictionaries.Remove(currentShapes);
            App.Current.Resources.MergedDictionaries.Add(newShapes);
            currentShapes = newShapes;

            // Changed
            return true;
        }
        #endregion // Public Methods
    }
}
