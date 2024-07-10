﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrackOMatic
{
    public partial class HintItemList : UserControl
    {
        private List<ItemName> sortedItemList = new();
        private List<bool> checkmarkedItems = new();
        public Dictionary<ItemName, bool> SelectedItems { get; set; } = new();

        public static readonly DependencyProperty CustomHorizontalAlignmentProperty =
            DependencyProperty.Register(
                "CustomHorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(HintItemList),
                new PropertyMetadata(HorizontalAlignment.Left)
            );

        public HorizontalAlignment CustomHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(CustomHorizontalAlignmentProperty); }
            set { SetValue(CustomHorizontalAlignmentProperty, value); }
        }

        public double[] SelectionDialogPosition { get; set; } = { 0, 0 };
        public HintInfo HintInfo { get; set; }
        public HintItemList()
        {
            InitializeComponent();
            DataContext = this;
            ProcessSelectedItems();

        }

        public void OpenItemSelectionDialog()
        {
            var dialog = new HintItemSelectionDialog(sortedItemList);

            var mousePosition = Mouse.GetPosition(this);
            mousePosition = PointToScreen(mousePosition);
            UIUtils.MoveWindowAndEnsureVisibile(dialog, mousePosition.X - 20, mousePosition.Y - 10);

            //dialog.Left = SelectionDialogPosition[0];
            //dialog.Top = SelectionDialogPosition[1];

            dialog.ShowDialog();
            var copy = new Dictionary<ItemName, bool>(dialog.SelectedItems);
            foreach(var entry in copy)
            {
                if (SelectedItems.ContainsKey(entry.Key))
                {
                    dialog.SelectedItems[entry.Key] = SelectedItems[entry.Key];
                }
            }
            SelectedItems = dialog.SelectedItems;
            ProcessSelectedItems();
            if (HintInfo != null) HintInfo.UpdateSelectedItems();
        }

        public void UpdateCheckmark(ItemName itemName, bool isChecked)
        {
            if (!SelectedItems.ContainsKey(itemName)) return;
            SelectedItems[itemName] = isChecked;
            if (HintInfo != null) HintInfo.UpdateSelectedItems();
        }

        public void Image_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed) OpenItemSelectionDialog();
        }

        public void AddNewImageToPanel(ItemName itemName, bool isChecked)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var resourceName = itemName.ToString().ToLower();
            var newItem = new PathOrFoundItem(itemName, isChecked, this);
            var validItem = mainWindow.ITEM_NAME_TO_ITEM.ContainsKey(itemName);
            if (HintInfo != null && HintInfo.HintType == HintType.DIRECT_ITEM_HINT && validItem)
            {
                var item = mainWindow.ITEM_NAME_TO_ITEM[itemName];
                if (!mainWindow.ITEM_TO_DIRECT_HINT.ContainsKey(itemName))
                {
                    mainWindow.ITEM_TO_DIRECT_HINT.Add(itemName, newItem);
                }
                else
                {
                    mainWindow.ITEM_TO_DIRECT_HINT[itemName] = newItem;
                }
                if (item.Parent == mainWindow.ItemGrid)
                {
                    mainWindow.ProcessNewAutotrackedItem(itemName, HintInfo.RegionName, true);
                }
            }
            newItem.MouseDown += Image_MouseDown;
            ItemPanel.Children.Add(newItem);
        }

        public void ProcessSelectedItems()
        {
            ItemPanel.Children.Clear();
            ItemPanel.BeginInit();
            sortedItemList = new();
            checkmarkedItems = new();
            foreach (var entry in SelectedItems)
            {
                var itemName = entry.Key;
                var isChecked = entry.Value;
                int index = (-1 * sortedItemList.BinarySearch(itemName)) - 1;
                sortedItemList.Insert(index, itemName);
                checkmarkedItems.Insert(index, isChecked);
            }
            if (sortedItemList.Count == 0)
            {
                sortedItemList.Add(ItemName.NONE);
                checkmarkedItems.Add(false);
            }
            for(int i = 0; i < sortedItemList.Count; ++i)
            {
                AddNewImageToPanel(sortedItemList[i], checkmarkedItems[i]);
            }
            ItemPanel.EndInit();
        }
    }
}
