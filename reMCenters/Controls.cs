using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace reMCenters
{
    class HoverableButton : Button
    {
        public Image ConnectedImage { get; set; }
        public float ThicknessDecrementValue = 5.0f;

        private Thickness OriginalMargin;
        public HoverableButton()
        {

            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;


        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (OriginalMargin == null) return;
            ConnectedImage.Margin = OriginalMargin;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var originalMargin = ConnectedImage.Margin;
            OriginalMargin = originalMargin;
            ConnectedImage.Margin = new Thickness(originalMargin.Left - ThicknessDecrementValue, originalMargin.Top - ThicknessDecrementValue, originalMargin.Right - ThicknessDecrementValue, originalMargin.Bottom - ThicknessDecrementValue);
        }
    }
}
