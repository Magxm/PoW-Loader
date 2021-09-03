using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PoW_ModLoader_Config
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point startPoint = new Point();
        private List<ModInfo> Mods = new List<ModInfo>();
        private int startIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            InitializeListView();
        }

        private void InitializeListView()
        {
            // Clear data
            modListView.Items.Clear();
            Mods.Clear();
            //Setting it to List View
            modListView.ItemsSource = Mods;
            //Parsing mod data
            _LoadMods();
        }

        private void _IteratePluginFolder(string folder)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(folder);

            foreach (string fileName in fileEntries)
            {
                if (Path.GetExtension(fileName) == ".dll")
                {
                    //We load the dlls assembly
                    if (!fileName.Contains("PoW_ModAPI"))
                    {
                        try
                        {
                            //We can do a reflection only load without needing to resolve the dll's dependencies
                            Assembly assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
                            bool isMod = false;
                            foreach (var assemblyName in assembly.GetReferencedAssemblies())
                            {
                                if (assemblyName.Name == "PoW_ModAPI")
                                {
                                    isMod = true;
                                    break;
                                }
                            }

                            if (isMod)
                            {
                                var mods = ModInfo.GetAllModsInDll(fileName);
                                foreach (var mod in mods)
                                {
                                    Mods.Add(mod);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.ToString());
                        }
                    }
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(folder);
            foreach (string subdirectory in subdirectoryEntries)
            {
                _IteratePluginFolder(subdirectory);
            }
        }

        private void _LoadMods()
        {
            Mods.Clear();
            var curDir = Directory.GetCurrentDirectory();
            var pluginDir = curDir + Path.DirectorySeparatorChar + "BepInEx" + Path.DirectorySeparatorChar + "plugins";
            _IteratePluginFolder(pluginDir);

            //First we find the highest index
            int highestLoadIndex = -1;
            foreach (var mod in Mods)
            {
                if (mod.LoadOrderIndex > highestLoadIndex)
                {
                    highestLoadIndex = mod.LoadOrderIndex;
                }
            }

            //And then we give all mods with a -1 load index the next highest one.
            foreach (var mod in Mods)
            {
                if (mod.LoadOrderIndex == -1)
                {
                    highestLoadIndex++;
                    mod.LoadOrderIndex = highestLoadIndex;
                }
            }

            //Then we check for double assignments
            //First we sort the list once.
            Mods = new List<ModInfo>(Mods.OrderBy((mod => mod.LoadOrderIndex)));
            //Then we check for and fix double assignments
            for (int i = 0; i < Mods.Count - 1; ++i)
            {
                if (Mods[i].LoadOrderIndex == Mods[i + 1].LoadOrderIndex)
                {
                    //We got a double assignment, we increase all following load orders by 1
                    for (int j = i + 1; j < Mods.Count; ++j)
                    {
                        Mods[j].LoadOrderIndex++;
                    }
                }
            }

            //Now we iterate once more and reassign the indices starting from 0, since in theory there could be holes
            for (int i = 0; i < Mods.Count; ++i)
            {
                Mods[i].LoadOrderIndex = i;
                Mods[i].Save();
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(Mods);
            view.Refresh();

            modListView.ItemsSource = Mods;
        }

        private void modListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get current mouse position
            startPoint = e.GetPosition(null);
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private bool InDragAndDropOperation = false;

        private void modListView_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                       Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                Debug.WriteLine("Here--");
                InDragAndDropOperation = true;
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null) return;           // Abort
                                                            // Find the data behind the ListViewItem
                ModInfo item = (ModInfo)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                if (item == null) return;                   // Abort
                                                            // Initialize the drag & drop operation
                startIndex = modListView.SelectedIndex;
                DataObject dragData = new DataObject("ModInfo", item);

                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private ListViewItem GetListViewItem(int index)
        {
            if (this.modListView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return this.modListView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        private bool IsMouseOver(Visual target)
        {
            // We need to use MouseUtilities to figure out the cursor coordinates because, during a
            // drag-drop operation, the WPF mechanisms for getting the coordinates behave strangely.

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = MouseUtilities.GetMousePosition(target);
            return bounds.Contains(mousePos);
        }

        private void modListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            // Update the item which is known to be currently under the drag cursor.
            int index = -1;
            for (int i = 0; i < this.Mods.Count; ++i)
            {
                ListViewItem item = this.GetListViewItem(i);
                if (this.IsMouseOver(item))
                {
                    index = i;
                    break;
                }
            }

            var ItemUnderDragCursor = index < 0 ? null : this.modListView.Items[index] as ModInfo;
        }

        private void modListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ModInfo") || sender != e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void UpdateModInfoEntries()
        {
            for (int i = 0; i < Mods.Count; ++i)
            {
                var modInfo = Mods[i];
                if (modInfo.LoadOrderIndex != i)
                {
                    modInfo.LoadOrderIndex = i;
                    modInfo.Save();
                }
            }

            ICollectionView view = CollectionViewSource.GetDefaultView(Mods);
            view.Refresh();
        }

        private void modListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("ModInfo") && sender == e.Source)
            {
                // Get the drop ListViewItem destination
                ListView listView = sender as ListView;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null)
                {
                    // Abort
                    e.Effects = DragDropEffects.None;
                    return;
                }
                // Find the data behind the ListViewItem
                ModInfo item = (ModInfo)listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                // Moving item in observable collection (this will be automatically reflected to modListView.ItemsSource)
                e.Effects = DragDropEffects.Move;
                int index = Mods.IndexOf(item);
                if (startIndex >= 0 && index >= 0)
                {
                    var modInfo = Mods[startIndex];
                    Mods.Remove(modInfo);
                    Mods.Insert(index, modInfo);
                }
                startIndex = -1;        // Done!
                InDragAndDropOperation = false;
                UpdateModInfoEntries();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _LoadMods();
        }
    }
}