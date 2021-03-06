﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Core.Entities
{
    public class EmployeeDetailTemp
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EID { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal BaseRate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ActualRate { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public string EmergencyFirstName { get; set; }
        public string EmergencyLastName { get; set; }
        public string EmergencyEmail { get; set; }
        public Nullable<int> EmergencyRelationship { get; set; }
        public string Mobile { get; set; }
        public string EmergencyMobile { get; set; }
        public Guid Role { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> ViewInvoice { get; set; }
        public Nullable<bool> MakeInvoice { get; set; }
        public Nullable<bool> ApproveInvoice { get; set; }
        public Nullable<bool> SendInvoice { get; set; }
        public Nullable<bool> MYOB { get; set; }
        public Nullable<bool> Employee { get; set; }
        public Nullable<bool> Contractor { get; set; }
        public string ABN { get; set; }
        public string TFN { get; set; }
        public string BusinessName { get; set; }
        public Nullable<Double> HourlyRate { get; set; }
        public Nullable<int> HomeAddressTitle { get; set; }
        public string HomeAddressFirstName { get; set; }
        public string HomeAddressLastName { get; set; }
        public string HomeAddressWork { get; set; }
        public string HomeAddressMobile { get; set; }
        public string HomeAddressLandLine { get; set; }
        public string HomeAddressEmail { get; set; }
        public string HomeAddressFax { get; set; }
        public string HomeAddressUnit { get; set; }
        public string HomeAddressStreetNo { get; set; }
        public string HomeAddressStreetName { get; set; }
        public Nullable<int> HomeAddressStreetType { get; set; }
        public string HomeAddressSuburb { get; set; }
        public string HomeAddressState { get; set; }
        public Nullable<int> HomeAddressPostalCode { get; set; }
        public Nullable<int> MailingAddressTitle { get; set; }
        public string MailingAddressFirstName { get; set; }
        public string MailingAddressLastName { get; set; }
        public string MailingAddressWork { get; set; }
        public string MailingAddressMobile { get; set; }
        public string MailingAddressLandLine { get; set; }
        public string MailingAddressEmail { get; set; }
        public string MailingAddressFax { get; set; }
        public string MailingAddressCO { get; set; }
        public Nullable<bool> MailingAddressC { get; set; }
        public Nullable<bool> MailingAddressPO { get; set; }
        public string MailingAddressUnit { get; set; }
        public string MailingAddressStreetNo { get; set; }
        public string MailingAddressStreetName { get; set; }
        public Nullable<int> MailingAddressSteetType { get; set; }
        public string MailingAddressSuburb { get; set; }
        public string MailingAddressState { get; set; }
        public Nullable<int> MailingAddressPostCode { get; set; }
        public string SignaturePicture { get; set; }
        public string SignaturePicDatabase { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePicDatabase { get; set; }
        public Nullable<double> MondayHrs { get; set; }
        public Nullable<double> TuesdayHrs { get; set; }
        public Nullable<double> WednesdayHrs { get; set; }
        public Nullable<double> ThursdayHrs { get; set; }
        public Nullable<double> FridayHrs { get; set; }
        public Nullable<double> Saturdayhrs { get; set; }
        public Nullable<double> SundayHrs { get; set; }
        public string VehicleChassisNo { get; set; }
        public Nullable<int> VehicleManufactringYear { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleEngine { get; set; }
        public string DrivingLicenseImg { get; set; }
        public string DrivingLicenseDatabase { get; set; }
        public Nullable<bool> HeightsSafetyTraining { get; set; }
        public Nullable<bool> TilelinkTraining { get; set; }
        public Nullable<bool> LadderlinkTraining { get; set; }
        public Nullable<bool> FroglinkTraining { get; set; }
        public Nullable<bool> WHSWhiteCard { get; set; }
        public Nullable<bool> FirstAidTraining { get; set; }
        public string BankDetailDoc { get; set; }
        public string BankDetailDatabase { get; set; }
        public string InsuranceDoc { get; set; }
        public string InsuranceDatabase { get; set; }
        public string BankBSB { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string FundName { get; set; }
        public string MemberNumber { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public Nullable<DateTime> EmpStartDate { get; set; }
        public Nullable<int> WorkType { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public List<string> GetUserRoles { get; set; }

    }
}
