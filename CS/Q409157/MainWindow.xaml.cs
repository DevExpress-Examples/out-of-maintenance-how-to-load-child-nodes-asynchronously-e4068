using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Grid;

namespace Q409157 {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
    }

    public class ViewModel {
        public ViewModel() {
            ItemsSource = DataGenerator.GenerateObjects(0);
        }

        public BindingList<TestObject> ItemsSource { get; set; }
    }

    public class TestObject {
        public int ID { get; set; }
        public string Name { get; set; }
        public BindingList<TestObject> Children { get; set; }
    }

    static public class DataGenerator {
        static int Count { get; set; }
        public static BindingList<TestObject> GenerateObjects(int id) {
            BindingList<TestObject> list = new BindingList<TestObject>();
            for(int i = 0; i < 300000; i++) {
                list.Add(new TestObject() { ID = i, Name = String.Format("TestObject{0}", Count) });
                Count++;
            }
            return list;
        }
    }

    public class MyChildSelector : IChildNodesSelector {
        public System.Collections.IEnumerable SelectChildren(object objectItem) {
            TestObject obj = objectItem as TestObject;
            if(obj != null) {
                BindingList<TestObject> list = new BindingList<TestObject>();
                FillChildren(list, obj);
                return list; ;
            }
            return null;
        }

        void FillChildren(BindingList<TestObject> list, TestObject obj) {
            BackgroundWorker bw = new BackgroundWorker();
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
            bw.DoWork += new DoWorkEventHandler((s, e) => {
                System.Threading.Thread.Sleep(3000);
                list.RaiseListChangedEvents = false;
                foreach(TestObject item in DataGenerator.GenerateObjects(obj.ID)) {
                    list.Add(item);
                }
                list.RaiseListChangedEvents = true;
                currentDispatcher.BeginInvoke(new Action(() => list.ResetBindings()));
            });
            bw.RunWorkerAsync();
        }
    }

}
