using FSM.Web.FSMConstant;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.ViewModels
{
    public class EmployeeDetailViewModel
    {
        [Key]
        public string EmployeeId { get; set; }
        public string EID { get; set; }
        [Required(ErrorMessage = "Please enter username")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "username field limit should be 3-50 characters")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [StringLength(50, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=^.{6,10}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$", ErrorMessage = "Password should contain atleast one uppercase character, one digit, one special character")]
        //[DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please enter confirm password")]
        [StringLength(50, ErrorMessage = "Confirm password must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Please enter email address")]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Please enter correct email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter first name")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "First name field limit should be 3-20 characters")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for first name.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter last name")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Last name field limit should be 3-20 characters")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for last name.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter mobile no.")]
       // [RegularExpression(@"^([0]|\+61[\-\s]?)?[789]\d{8,9}$", ErrorMessage = "Entered mobile no is not valid.")]
        public string Mobile { get; set; }
        [DisplayName("Emergency First Name")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Last name field limit should be 3-20 characters")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for last name.")]
        public string EmergencyFirstName { get; set; }
        [DisplayName("Emergency Last Name")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Last name field limit should be 3-20 characters")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for last name.")]
        public string EmergencyLastName { get; set; }
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Please enter correct emergency email address")]
        public string EmergencyEmail { get; set; }

        [DisplayName("Emergency Mobile")]
        [RegularExpression(@"^([0]|\+91[\-\s]?)?[789]\d{9}$", ErrorMessage = "Entered Emergency Mobile No is not valid.")]
        
        public string EmergencyMobile { get; set; }
        public Nullable<Constant.RelationShip> EmergencyRelationship { get; set; }
        [Required(ErrorMessage = "Please select role")]
        public Guid Role { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public bool IsActive { get; set; }
        public bool ViewInvoice { get; set; }
        public bool MakeInvoice { get; set; }
        public bool ApproveInvoice { get; set; }
        public bool SendInvoice { get; set; }
        public bool MYOB { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for TFN.")]
        [StringLength(50)]
        public string TFN { get; set; }

        public bool Employee { get; set; }
        public bool Contractor { get; set; }
        public string ABN { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for BusinessName.")]
        [StringLength(50)]
        public string BusinessName { get; set; }
       
        public Nullable<double> HourlyRate { get; set; }
        public Nullable<Constant.HomeAddressTitle> HomeAddressTitle { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only  for First Name in Home Address.")]
        [StringLength(50)]
        public string HomeAddressFirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only for Last Name in Home Address.")]
        [StringLength(50)]
        public string HomeAddressLastName { get; set; }
        [StringLength(100)]
        public string HomeAddressWork { get; set; }
        [RegularExpression(@"^([0]|\+91[\-\s]?)?[789]\d{9}$", ErrorMessage = "Entered Mobile No is not valid.")]
        public string HomeAddressMobile { get; set; }
        [StringLength(20, ErrorMessage = "Home address Landline Maximum 20 characters long.")]
        public string HomeAddressLandLine { get; set; }
        [StringLength(50)]
        public string HomeAddressEmail { get; set; }
        [StringLength(50)]
        public string HomeAddressFax { get; set; }
        [StringLength(10)]
        public string HomeAddressUnit { get; set; }
        [StringLength(20)]
        public string HomeAddressStreetNo { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for home address street name.")]
        [StringLength(50)]
        public string HomeAddressStreetName { get; set; }
        public Nullable<Constant.HomeAddressStreetType> HomeAddressStreetType { get; set; }
        [StringLength(50)]
        public string HomeAddressSuburb { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for home State Address.")]
        [StringLength(50)]
        public string HomeAddressState { get; set; }
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        public string HomeAddressPostalCode { get; set; }
        public Nullable<Constant.MailingAddressTitle> MailingAddressTitle { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only for First Name in Mailing Address.")]
        [StringLength(50)]
        public string MailingAddressFirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only for Last Name in Mailing Address.")]
        [StringLength(50)]
        public string MailingAddressLastName { get; set; }
        [StringLength(100)]
        public string MailingAddressWork { get; set; }
        [RegularExpression(@"^([0]|\+91[\-\s]?)?[789]\d{9}$", ErrorMessage = "Entered Mobile No is not valid.")]
        public string MailingAddressMobile { get; set; }
        public string MailingAddressLandLine { get; set; }
        [StringLength(50)]
        public string MailingAddressEmail { get; set; }
        [StringLength(20)]
        public string MailingAddressFax { get; set; }
        [StringLength(20)]
        public string MailingAddressCO { get; set; }
        public Nullable<bool> MailingAddressC { get; set; }
        public Nullable<bool> MailingAddressPO { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for Mailing address unit.")]
        [StringLength(10)]
        public string MailingAddressUnit { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for stree no address.")]
        [StringLength(20)]
        public string MailingAddressStreetNo { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for mailing state name.")]
        [StringLength(50)]
        public string MailingAddressStreetName { get; set; }
        public Nullable<Constant.MailingAddressSteetType> MailingAddressSteetType { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Use letters only for Street Name in mailing address Surub.")]
        [StringLength(50)]
        public string MailingAddressSuburb { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for mailing State Address.")]
        [StringLength(50)]
        public string MailingAddressState { get; set; }
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Enter Digits 4 valid for postal code.")]
        public string MailingAddressPostCode { get; set; }
        public string SignaturePicture { get; set; }
        public string ProfilePicture { get; set; }
        [Range(0, 24, ErrorMessage = "The value must be between 0 and 24")]
        public Nullable<double> MondayHrs { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for tuesday hrs.")]
        [Range(0, 24, ErrorMessage = "The value must be between 0 and 24")]
        public Nullable<double> TuesdayHrs { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for wednesday hrs.")]
        [Range(0, 24, ErrorMessage = "The value must be between 0 and 24")]
        public Nullable<double> WednesdayHrs { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for thursday hrs.")]
        [Range(0, 24, ErrorMessage = "The value must be between 0 and 24")]
        public Nullable<double> ThursdayHrs { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for friday hrs.")]
        [Range(0, 24, ErrorMessage = "The value must be between 0 and 24")]
        public Nullable<double> FridayHrs { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for saturday hrs.")]
        [Range(0, 24, ErrorMessage = "The value must be between 0 and 24")]
        public Nullable<double> Saturdayhrs { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Special characters not allowed for sunday hrs.")]
        [Range(0, 24, ErrorMessage = "The value must be between 0 and 24")]
        public Nullable<double> SundayHrs { get; set; }
        public string SignaturePictureTemp { get; set; } // temp variable to hold signature pic
        public string ProfilePictureTemp { get; set; } // temp variable to hold profile pic
        [DisplayName("Chassis No.")]
        public string VehicleChassisNo { get; set; }
        [DisplayName("Manufacture Year")]
        public Nullable<DateTime> VehicleManufactringYear { get; set; }
        [DisplayName("Model")]
        public string VehicleModel { get; set; }
        [DisplayName("Engine")]
        public string VehicleEngine { get; set; }
        [DisplayName("Date Checked")]
        public Nullable<DateTime> VehicleDateChecked { get; set; }
        public bool HeightsSafetyTraining { get; set; }
        public bool TilelinkTraining { get; set; }
        public bool LadderlinkTraining { get; set; }
        public bool FroglinkTraining { get; set; }
        public bool WHSWhiteCard { get; set; }
        public bool FirstAidTraining { get; set; }
        [DisplayName("Driving License")]
        public string DrivingLicenseImg { get; set; }
        public HttpPostedFileBase DrivingLicense { get; set; }
        [DisplayName("Bank Detail")]
        public string BankDetailDoc { get; set; }
        public HttpPostedFileBase BankDetail { get; set; }
        [DisplayName("Insurance")]
        public string InsuranceDoc{ get; set; }
        public HttpPostedFileBase Insurance { get; set; }
        public string SignaturePicDatabase { get; set; }
        public HttpPostedFileBase SignatureDoc { get; set; }
        public string ProfilePicDatabase { get; set; }
        public HttpPostedFileBase ProfileDoc { get; set; }
        public string DrivingLicenseDatabase { get; set; }
        public string BankDetailDatabase { get; set; }
        public string InsuranceDatabase { get; set; }
        public string BankBSB { get; set; }
        public string BankAccount { get; set; }

        public string BankName { get; set; }
        public string FundName { get; set; }
        public string MemberNumber { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public Nullable<DateTime> EmpStartDate { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select work type")]
        public Nullable<Constant.WorkType> WorkType { get; set; }
        [Required(ErrorMessage = "Please select work type")]
        [DisplayName("Select Work Type")]
        public List<Nullable<int>> WorkTypeId { get; set; }

        [DisplayName("Upload Files")]
        public Nullable<Constant.UploadFiles> UploadFiles { get; set; }

        public Nullable<Guid> CategoryId { get; set; }
        public Nullable<Guid> SubCategoryId { get; set; }
        public IEnumerable<SelectListItem> RateCategoryList { get; set; }
        public IEnumerable<SelectListItem> RateSubCategoryList { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal BaseRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ActualRate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> EffectiveDate { get; set; }
        public List<SelectListItem> WorkTypeList { get; set; }
        public bool IsDelete { get; set; } 
        public string OrderType { get; set; }
        public Nullable<int> OTRW_Order { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public List<string> GetUserRoles { get; set; }
        public string ModifyUserName { get; set; }
    }
}