using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Module.BO {
    [DomainComponent]
    public interface IBaseDomainComponent : ISupportSequentialNumber { }

    //Dennis: Unfortunately, the SequentialNumber property will not will not work untill the "B184328 - DC - Overridden interface property is not initialized when a base persistent class has the same property" is fixed.
    //Dennis: Thus, I am hiding this entity from the navigation system of the application.
    //[DefaultClassOptions]
    [DomainComponent]
    [XafDefaultProperty("Title")]
    [ImageName("BO_Note")]
    public interface IDocument : IBaseDomainComponent {
        [Calculated("concat('D', ToStr(SequentialNumber))")]
        string DocumentId { get; }
        [RuleRequiredField("IDocument.Title.RuleRequiredField", DefaultContexts.Save)]
        [FieldSize(255)]
        string Title { get; set; }
        [FieldSize(4000)]
        string Text { get; set; }
        [PersistentDc, Browsable(false)]
        new long SequentialNumber { get; set; }
    }
}