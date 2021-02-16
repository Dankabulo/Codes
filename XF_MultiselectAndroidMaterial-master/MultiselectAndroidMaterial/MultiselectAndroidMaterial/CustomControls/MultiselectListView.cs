using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MultiselectAndroidMaterial.CustomControls
{
    public class MultiselectListView : ListView
    {
        public static readonly BindableProperty SelectionColorProperty =
                BindableProperty.Create(nameof(SelectionColor),
                                        typeof(string),
                                        typeof(MultiselectListView),
                                        "#d8d8d8");
        public string SelectionColor
        {
            get { return GetValue(SelectionColorProperty).ToString(); }
            set { SetValue(SelectionColorProperty, value); }
        }
        public MultiselectListView()
        {
            this.ItemTemplate = new DataTemplate(typeof(MultiselectViewCell));
            this.ItemTapped += MultiselectListView_ItemTapped;
        }

        private void MultiselectListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedItem = (MultiselectListViewItem)e.Item;
            if (selectedItem.Selected)
            {
                selectedItem.Selected = false;
                selectedItem.SelectedColor = "#ffffff";
            }
            else
            {
                selectedItem.Selected = true;
                selectedItem.SelectedColor = SelectionColor;
            }
            this.ItemTemplate = new DataTemplate(typeof(MultiselectViewCell));
        }

        /// <summary>
        /// Get the selected items
        /// </summary>
        /// <returns></returns>
        public List<MultiselectListViewItem> SelectedItems()
        {
            if(this.ItemsSource!= null)
            {
                return ((List<MultiselectListViewItem>)this.ItemsSource).FindAll(item => item.Selected);
            }
            return null;
        }
    }

    /// <summary>
    /// Listview tempalte
    /// </summary>
    public class MultiselectViewCell : ViewCell
    {
        public MultiselectViewCell()
        {
            /* Rounded Icon with the First letter of the Text*/
            // I used a rounded button render I implemented before, but I know we could use a more simple control
            // as a stacklayout with a label inside it.
            RoundedButton roundedBtn = new RoundedButton();
            roundedBtn.SetBinding(RoundedButton.TextProperty, "TextInitial");
            roundedBtn.VerticalOptions = LayoutOptions.Center;
            roundedBtn.TextColor = Color.White;
            roundedBtn.BorderRadius = 120;
            roundedBtn.WidthRequest = 40;
            roundedBtn.HeightRequest = 40;
            roundedBtn.Margin = new Thickness(16, 8, 16, 8);
            roundedBtn.SetBinding(RoundedButton.BackgroundColorProperty, new Binding("CyrcleColor", BindingMode.OneWay));


            /* Badge - source code from : https://gist.github.com/rudyryk/8cbe067a1363b45351f6 */
            // On this way the refresh is a bit slow and blink all the listView (researching)

            //StackLayout cyrcleCont = new StackLayout();
            //cyrcleCont.VerticalOptions = LayoutOptions.Center;
            //Badge cyrcleIcon = new Badge(40,15);
            //cyrcleIcon.SetBinding(Badge.TextProperty, "TextInitial");
            //cyrcleIcon.VerticalOptions = LayoutOptions.Center;
            //cyrcleIcon.Margin = new Thickness(16, 8, 16, 8);
            ////cyrcleIcon.SetBinding(Badge.IsVisibleProperty, "BadgeIsVisible");
            //cyrcleIcon.SetBinding(Badge.BoxColorProperty, new Binding("CyrcleColor", BindingMode.OneWay));
            //cyrcleCont.Children.Add(cyrcleIcon);


            /* Label to show the main Text */
            var textLabel = new Label();
            textLabel.SetBinding(Label.TextProperty, "Text");
            textLabel.VerticalOptions = LayoutOptions.Center;
            textLabel.HorizontalOptions = LayoutOptions.FillAndExpand;            

            /* Container for the previous views */
            var stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal; // Element horizontal anordnen            
            stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            stack.SetBinding(StackLayout.BackgroundColorProperty, new Binding("SelectedColor", BindingMode.OneWay));

            //stack.Children.Add(cyrcleCont);
            stack.Children.Add(roundedBtn);
            stack.Children.Add(textLabel);
            this.View = stack;
        }
    }

    /// <summary>
    /// Listview items
    /// </summary>
    public class MultiselectListViewItem
    {
        public string Text { get; set; }
        public string TextInitial { get; set; }
        public bool Selected { get; set; }
        public string SelectedColor { get; set; }
        public string CyrcleColor { get; set; } 

        public MultiselectListViewItem(string text)
        {
            this.Text = text;
            this.TextInitial = text.Substring(0, 1);
            this.SelectedColor = "#ffffff";
            this.CyrcleColor = "#6666ff";
        }

        /* Unused method by now */
        public string GetRandomColorString()
        {
            var random = new Random();
            return String.Format("#{0:X6}", random.Next(0x1000000));
        }
        
    }

    
}
