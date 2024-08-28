﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Data;
using TrackOMatic.Properties;
using System.ComponentModel;

namespace TrackOMatic
{
    public partial class MainWindow : Window
    {
        private void TopMostToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.TopMost = TopMostOption.IsChecked;
            Topmost = TopMostOption.IsChecked;
            Settings.Default.Save();
        }

        private void HitListToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.HitList = HitListOption.IsChecked;
            ResetWidthHeight();
            Settings.Default.Save();
        }

        private void SongDisplayToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.SongDisplay = SongDisplayOption.IsChecked;
            Settings.Default.Save();
            if(BroadcastView != null)
            {
                BroadcastView.UpdateSongDisplayHeight();
            }
        }

        private void HintDisplayToggle(object sender, RoutedEventArgs e)
        {
            var button = (sender as RadioButton);
            Settings.Default.HintDisplay = button.Content.ToString();
            ResetWidthHeight();
            UpdateHintDisplayToggles();
            Settings.Default.Save();
        }

        private void AutotrackToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.Autotracking = AutotrackOption.IsChecked;
            /*
            if (!AutotrackOption.IsChecked) 
            {
                foreach (var item in DraggableItems.Cast<Item>())
                {
                    item.IsEnabled = true;
                    item.CanLeftClick = true;
                }
                DataSaver.TurnOffAutotrackingField();
                foreach (var entry in Regions) SetRegionLighting(entry.Key, false);
            }
            else
            {
                Autotracker.ResetChecks();
            }*/
            Settings.Default.Save();
        }

        private void BroadcastToggle(object sender, RoutedEventArgs e)
        {
            if(BroadcastView != null)
            {
                BroadcastView.Close();
                BroadcastView = null;
                return;
            }
            BroadcastView = new BroadcastView();
            BroadcastView.UpdateSongInfo(SongGame.Text, SongName.Text);
            BroadcastView.Closed += BroadcastClosed;
            BroadcastView.Show();
            BroadcastView.InitializeFromItems(ITEM_NAME_TO_ITEM);
            foreach (var entry in Collectibles)
            {
                BroadcastView.UpdateCollectible(entry.Key, entry.Value.Text);
            }
            foreach (var entry in Regions)
            {
                var region = entry.Value;
                if (region.LevelOrderNumber != null) region.LevelOrderNumber.UpdateLabel();
                region.UpdatePoints();
            }
            BroadcastView.ProcessSpoilerSettings(SpoilerSettings);
            BroadcastView.SetShopkeepers(ShopkeeperColumn.ActualWidth > 0);
            BroadcastOption.IsChecked = true;
        }
    }
}
