using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using GenerateUserFriendlyId.Module.BO;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Module.Controllers {
    public partial class ImportDataListViewController : ViewController {
        public ImportDataListViewController() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void saImportData_Execute(object sender, SimpleActionExecuteEventArgs e) {
            ImportData();
        }
        private void ImportData() {
            Type currentObjectType = View.ObjectTypeInfo.Type;
            using (IObjectSpace importObjectSpace = Application.CreateObjectSpace()) {
                try {
                    for (int i = 0; i < 20; i++) {
                        if (currentObjectType == typeof(Contact))
                            DatabaseHelper.CreateContact(importObjectSpace);
                        if (currentObjectType == typeof(Address))
                            DatabaseHelper.CreateAddress(importObjectSpace);
                        if (currentObjectType == typeof(IDocument))
                            DatabaseHelper.CreateDocument(importObjectSpace);
                    }
                    importObjectSpace.CommitChanges();
                } catch {
                    importObjectSpace.Rollback();
                    throw;
                }
            }
            View.ObjectSpace.Refresh();
        }
    }
}