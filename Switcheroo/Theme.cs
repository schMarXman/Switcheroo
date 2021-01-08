using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Switcheroo
{
    [Serializable]
    public class Theme
    {
        #region SearchBox
        public string SearchBoxBackgroundColor { get; set; } = string.Empty;
        public string SearchBoxFontColor { get; set; } = string.Empty;
        public string SearchBoxBorderColor { get; set; } = string.Empty;
        public string SearchBoxTextSelectionColor { get; set; } = string.Empty;
        #endregion

        #region ProcessList
        public string ProcessListBackgroundColor { get; set; } = string.Empty;
        public string ProcessListBorderColor { get; set; } = string.Empty;
        #endregion

        #region ProcessItem
        public string ProcessItemBackgroundColor { get; set; } = string.Empty;
        public string ProcessItemFontColor { get; set; } = string.Empty;
        public string ProcessItemFadeOutColor { get; set; } = string.Empty;
        public string ProcessItemClosingColor { get; set; } = string.Empty;
        #endregion

        #region HelpLabels
        public string FocusWindowHelpLabelFontColor { get; set; } = string.Empty;
        public string CloseWindowHelpLabelFontColor { get; set; } = string.Empty;
        public string NavigationHelpLabelFontColor { get; set; } = string.Empty;
        public string SearchHelpLabelFontColor { get; set; } = string.Empty;
        public string DismissHelpLabelFontColor { get; set; } = string.Empty;
        #endregion
        public static Theme GetDefaultLightTheme()
        {
            return new Theme()
            {
                SearchBoxBackgroundColor = "#FFFFFFFF",
                SearchBoxFontColor = "#FF000000",
                SearchBoxTextSelectionColor = "#FFFFFFFF",
                SearchBoxBorderColor = "#FF808080",

                ProcessListBackgroundColor = "#FFFFFFFF",
                ProcessListBorderColor = "#FF808080",

                ProcessItemBackgroundColor = "#FFFFFFFF",
                ProcessItemFontColor = "#FF000000",
                ProcessItemFadeOutColor = "#FF808080",
                ProcessItemClosingColor = "#FF808080",

                SearchHelpLabelFontColor = "#FF000000",
                NavigationHelpLabelFontColor = "#FF000000",
                FocusWindowHelpLabelFontColor = "#FF000000",
                CloseWindowHelpLabelFontColor = "#FF000000",
                DismissHelpLabelFontColor = "#FF000000",
            };
        }

        public static Theme GetDefaultDarkTheme()
        {
            return new Theme()
            {
                SearchBoxBackgroundColor = "#FF000000",
                SearchBoxFontColor = "#FFFFFFFF",
                SearchBoxTextSelectionColor = "#FFFFFFFF",
                SearchBoxBorderColor = "#FF808080",

                ProcessListBackgroundColor = "#FF000000",
                ProcessListBorderColor = "#FF808080",

                ProcessItemBackgroundColor = "#FF000000",
                ProcessItemFontColor = "#FFFFFFFF",
                ProcessItemFadeOutColor = "#FFFFFFFF",
                ProcessItemClosingColor = "#FFFFFFFF",

                SearchHelpLabelFontColor = "#FFFFFFFF",
                NavigationHelpLabelFontColor = "#FFFFFFFF",
                FocusWindowHelpLabelFontColor = "#FFFFFFFF",
                CloseWindowHelpLabelFontColor = "#FFFFFFFF",
                DismissHelpLabelFontColor = "#FFFFFFFF",
            };
        }
    }
}
