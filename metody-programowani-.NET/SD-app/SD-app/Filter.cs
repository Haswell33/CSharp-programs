using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    class Filter
    {
        private List<List<Label>> head_labels = new List<List<Label>>();
        private List<List<Label>> all_labels = new List<List<Label>>();
        double count_row;
        public Filter(List<string> title_list, List<List<string>> row_list, Grid cur_grid, EventHandler<string> new_event)
        {
            double cur_Width = 1000;

            //Tytle
            StackPanel new_stack_for_head = new StackPanel
            {
                Height = 30,
                VerticalAlignment = 0,
                Background = new SolidColorBrush(Color.FromRgb(120, 120, 120)),
                Orientation = 0,
                Margin = new Thickness(0, 0, 10, 0)
            };
            count_row = title_list.Count() + 3;
            double margin_in_row = cur_Width / (title_list.Count() + 2);
            List<Label> cur_labels_head = new List<Label>();
            foreach (string one_row in title_list)
            {
                Label new_label = new Label
                {
                    Content = one_row,
                    Width = margin_in_row
                };
                new_stack_for_head.Children.Add(new_label);
                cur_labels_head.Add(new_label);
            }
            head_labels.Add(cur_labels_head);
            cur_grid.Children.Add(new_stack_for_head);

            //Content
            double margin_panel = 40;
            byte color_row = 160;

            margin_in_row = cur_Width / (title_list.Count() + 4);
            foreach (List<string> one_row in row_list)
            {
                StackPanel new_stack = new StackPanel
                {
                    Height = 30,
                    VerticalAlignment = 0,
                    Background = new SolidColorBrush(Color.FromRgb(color_row, color_row, color_row)),
                    Orientation = 0,
                    Margin = new Thickness(0, margin_panel, 10, 0)
                };
                _ = color_row == 160 ? color_row = 210 : color_row = 160;

                margin_panel += 40;
                List<Label> cur_labels = new List<Label>();
                int i = 0;
                foreach (string one_element in one_row)
                {
                    Label new_label = new Label
                    {
                        Content = one_element,
                        Width = margin_in_row
                    };
                    new_stack.Children.Add(new_label);
                    cur_labels.Add(new_label);
                    i++;
                }
                all_labels.Add(cur_labels);

                Button new_button_to_row = new Button
                {
                    Content = "Click to open",
                    Width = 110,
                    Height = 30,
                    HorizontalAlignment = (HorizontalAlignment)2,
                    Margin = new Thickness(10, 0, 10, 0)

                };
                new_button_to_row.Click += (sender, EventArgs) => { new_event?.Invoke(sender, one_row[title_list.IndexOf("ID")]); };

                new_stack.Children.Add(new_button_to_row);
                cur_grid.Children.Add(new_stack);

            }
        }
        public void change_window_size(double cur_Height, double cur_Width)
        {

            double margin_in_row = cur_Width / count_row;
            foreach (List<Label> cur_labels_list in head_labels)
            {
                foreach (Label cur_label in cur_labels_list)
                {
                    cur_label.Width = margin_in_row;
                }
            }
            margin_in_row = cur_Width / count_row;
            foreach (List<Label> cur_labels_list in all_labels)
            {
                foreach (Label cur_label in cur_labels_list)
                {
                    cur_label.Width = margin_in_row;
                }
            }
        }
    }
}
