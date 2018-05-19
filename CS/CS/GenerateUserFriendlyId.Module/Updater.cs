using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Module {
    public class Updater : ModuleUpdater {
        public Updater(ObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            SequenceGenerator.RegisterSequences(XafTypesInfo.Instance.PersistentTypes);
        }
    }
}
