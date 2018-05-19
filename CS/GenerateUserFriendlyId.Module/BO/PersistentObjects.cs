using System;
using DevExpress.Xpo;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Module.BO {
    public abstract class BasePersistentObject : BaseObject, ISupportSequentialNumber {
        private long _SequentialNumber;
        private static SequenceGenerator sequenceGenerator;
        public BasePersistentObject(Session session)
            : base(session) {
        }
        protected override void OnSaving() {
            base.OnSaving();
            if (Session.IsNewObject(this) && !typeof(NestedUnitOfWork).IsInstanceOfType(Session))
                GenerateSequence();
        }
        public void GenerateSequence() {
            if (sequenceGenerator == null)
                sequenceGenerator = new SequenceGenerator();
            OnSequenceGenerated(sequenceGenerator.GetNextSequence(ClassInfo));
            if (!(Session is NestedUnitOfWork))
                Session.AfterCommitTransaction += Session_AfterCommitTransaction;
        }
        private void Session_AfterCommitTransaction(object sender, SessionManipulationEventArgs e) {
            Session session = (Session)sender;
            if (sequenceGenerator != null) {
                try {
                    sequenceGenerator.Accept();
                } finally {
                    session.AfterCommitTransaction -= Session_AfterCommitTransaction;
                    sequenceGenerator.Dispose();
                    sequenceGenerator = null;
                }
            }
        }
        protected virtual void OnSequenceGenerated(long newId) {
            _SequentialNumber = newId;
        }
        [Browsable(false)]
        public long SequentialNumber {
            get { return _SequentialNumber; }
            set { SetPropertyValue("SequentialNumber", ref _SequentialNumber, value); }
        }
    }
    [DefaultClassOptions]
    [DefaultProperty("FullAddress")]
    public class Address : BasePersistentObject {
        string zipCode;
        string country;
        string province;
        string city;
        string address1;
        string address2;
        [PersistentAlias("concat('A', ToStr(SequentialNumber))")]
        public string AddressId {
            get {
                return Convert.ToString(EvaluateAlias("AddressId"));
            }
        }
        public string Province {
            get {
                return province;
            }
            set {
                SetPropertyValue("Province", ref province, value);
            }
        }
        public string ZipCode {
            get {
                return zipCode;
            }
            set {
                SetPropertyValue("ZipCode", ref zipCode, value);
            }
        }
        public string Country {
            get {
                return country;
            }
            set {
                SetPropertyValue("Country", ref country, value);
            }
        }
        public string City {
            get {
                return city;
            }
            set {
                SetPropertyValue("City", ref city, value);
            }
        }
        public string Address1 {
            get {
                return address1;
            }
            set {
                SetPropertyValue("Address1", ref address1, value);
            }
        }
        public string Address2 {
            get {
                return address2;
            }
            set {
                SetPropertyValue("Address2", ref address2, value);
            }
        }
        [Association]
        public XPCollection<Contact> Persons {
            get {
                return GetCollection<Contact>("Persons");
            }
        }
        [PersistentAlias("concat(Country, Province, City, ZipCode)")]
        public string FullAddress {
            get { return ObjectFormatter.Format("{Country}; {Province}; {City}; {ZipCode}", this, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty); }
        }
        public Address(Session session) : base(session) { }
    }
    [DefaultClassOptions]
    [DefaultProperty("FullName")]
    public class Contact : BasePersistentObject {
        string firstName;
        string lastName;
        Sex sex;
        int age;
        Address address;
        [PersistentAlias("concat('C', ToStr(SequentialNumber))")]
        public string ContactId {
            get {
                return Convert.ToString(EvaluateAlias("ContactId"));
            }
        }
        public string FirstName {
            get {
                return firstName;
            }
            set {
                SetPropertyValue("FirstName", ref firstName, value);
            }
        }
        public string LastName {
            get {
                return lastName;
            }
            set {
                SetPropertyValue("LastName", ref lastName, value);
            }
        }
        public int Age {
            get {
                return age;
            }
            set {
                SetPropertyValue("Age", ref age, value);
            }
        }
        public Sex Sex {
            get {
                return sex;
            }
            set {
                SetPropertyValue("Sex", ref sex, value);
            }
        }
        [Association]
        public Address Address {
            get {
                return address;
            }
            set {
                SetPropertyValue("Address", ref address, value);
            }
        }
        [PersistentAlias("concat(FirstName, LastName)")]
        public string FullName {
            get { return ObjectFormatter.Format("{FirstName} {LastName}", this, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty); }
        }
        public Contact(Session session) : base(session) { }
    }
    public enum Sex {
        Male,
        Female
    }
}