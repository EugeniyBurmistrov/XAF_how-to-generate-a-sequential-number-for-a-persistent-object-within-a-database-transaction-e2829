using System;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using GenerateUserFriendlyId.Module.Utils;

namespace GenerateUserFriendlyId.Module.BO {
    //Dennis: Uncomment this code if you want to have the SequentialNumber column created in each derived class table.
    [NonPersistent]
    public abstract class BasePersistentObject : BaseObject, ISupportSequentialNumber {
        private long _SequentialNumber;
        private static SequenceGenerator sequenceGenerator;
        private static object syncRoot = new object(); 
        public BasePersistentObject(Session session)
            : base(session) {
        }
        protected override void OnSaving() {
            try {
                base.OnSaving();
                if (Session.IsNewObject(this) && !typeof(NestedUnitOfWork).IsInstanceOfType(Session))
                    GenerateSequence();
            } catch {
                CancelSequence();
                throw;
            }
        }
        public void GenerateSequence() {
            lock (syncRoot) {
                Dictionary<string, bool> typeToExistsMap = new Dictionary<string, bool>();
                foreach (object item in Session.GetObjectsToSave())
                    typeToExistsMap[Session.GetClassInfo(item).FullName] = true;
                if (sequenceGenerator == null)
                    sequenceGenerator = new SequenceGenerator(typeToExistsMap);
                SubscribeToEvents();
                OnSequenceGenerated(sequenceGenerator.GetNextSequence(ClassInfo));
            }
        }
        private void AcceptSequence() {
            lock (syncRoot) {
                if (sequenceGenerator != null) {
                    try {
                        sequenceGenerator.Accept();
                    } finally {
                        CancelSequence();
                    }
                }
            }
        }
        private void CancelSequence() {
            lock (syncRoot) {
                UnSubscribeFromEvents();
                if (sequenceGenerator != null) {
                    sequenceGenerator.Close();
                    sequenceGenerator = null;
                }
            }
        }
        private void Session_AfterCommitTransaction(object sender, SessionManipulationEventArgs e) {
            AcceptSequence();
        }
        private void Session_AfterRollBack(object sender, SessionManipulationEventArgs e) {
            CancelSequence();
        }
        private void Session_FailedCommitTransaction(object sender, SessionOperationFailEventArgs e) {
            CancelSequence();
        }
        private void SubscribeToEvents() {
            if (!(Session is NestedUnitOfWork)) {
                Session.AfterCommitTransaction += Session_AfterCommitTransaction;
                Session.AfterRollbackTransaction += Session_AfterRollBack;
                Session.FailedCommitTransaction += Session_FailedCommitTransaction;
            }
        }
        private void UnSubscribeFromEvents() {
            if (!(Session is NestedUnitOfWork)) {
                Session.AfterCommitTransaction -= Session_AfterCommitTransaction;
                Session.AfterRollbackTransaction -= Session_AfterRollBack;
                Session.FailedCommitTransaction -= Session_FailedCommitTransaction;
            }
        }
        private void OnSequenceGenerated(long newId) {
            SequentialNumber = newId;
        }
        [Browsable(false)]
        //Dennis: Comment out this code if you do not want to have the SequentialNumber column created in each derived class table.
        [Indexed(Unique = false)]
        public long SequentialNumber {
            get { return _SequentialNumber; }
            set { SetPropertyValue("SequentialNumber", ref _SequentialNumber, value); }
        }
    }
    [DefaultClassOptions]
    [DefaultProperty("FullAddress")]
    [ImageName("BO_Address")]
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
    [ImageName("BO_Person")]
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