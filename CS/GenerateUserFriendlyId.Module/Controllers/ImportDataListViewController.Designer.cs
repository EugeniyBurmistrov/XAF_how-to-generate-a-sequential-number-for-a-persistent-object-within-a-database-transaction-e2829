namespace GenerateUserFriendlyId.Module.Controllers {
    partial class ImportDataListViewController {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.saImportData = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // saImportData
            // 
            this.saImportData.Caption = "Click to import data";
            this.saImportData.ConfirmationMessage = null;
            this.saImportData.Id = "ImportData";
            this.saImportData.ImageName = "Attention";
            this.saImportData.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
            this.saImportData.Shortcut = null;
            this.saImportData.Tag = null;
            this.saImportData.TargetObjectsCriteria = null;
            this.saImportData.TargetViewId = null;
            this.saImportData.ToolTip = null;
            this.saImportData.TypeOfView = null;
            this.saImportData.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.saImportData_Execute);
            // 
            // ImportDataListViewController
            // 
            this.TargetObjectType = typeof(GenerateUserFriendlyId.Module.Utils.ISupportSequentialNumber);
            this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.TypeOfView = typeof(DevExpress.ExpressApp.ListView);

        }
       #endregion
        private DevExpress.ExpressApp.Actions.SimpleAction saImportData;
    }
}
