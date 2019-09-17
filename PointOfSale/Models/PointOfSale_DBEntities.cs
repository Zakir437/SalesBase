namespace PointOfSale.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PointOfSale_DBEntities : DbContext
    {
        public PointOfSale_DBEntities()
            : base("name=PointOfSale_DBEntities")
        {
        }

        public virtual DbSet<AccNameAssignedWithUser> AccNameAssignedWithUsers { get; set; }
        public virtual DbSet<AccountAssignedWithMethod> AccountAssignedWithMethods { get; set; }
        public virtual DbSet<AccountName> AccountNames { get; set; }
        public virtual DbSet<AccType> AccTypes { get; set; }
        public virtual DbSet<AfterSaleService> AfterSaleServices { get; set; }
        public virtual DbSet<AmountCoupon> AmountCoupons { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<CountryList> CountryLists { get; set; }
        public virtual DbSet<CountryOfOrigin_Table> CountryOfOrigin_Table { get; set; }
        public virtual DbSet<CreditLimit> CreditLimits { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DailyAttendance> DailyAttendances { get; set; }
        public virtual DbSet<DebitLimit> DebitLimits { get; set; }
        public virtual DbSet<DefaultAccount> DefaultAccounts { get; set; }
        public virtual DbSet<DeliveryCharge> DeliveryCharges { get; set; }
        public virtual DbSet<DeliveryChargeCoupon> DeliveryChargeCoupons { get; set; }
        public virtual DbSet<DiscountItem> DiscountItems { get; set; }
        public virtual DbSet<DivisionList> DivisionLists { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<EmpSalaryPayment> EmpSalaryPayments { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<GenerateDate> GenerateDates { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<HolidayPackageList> HolidayPackageLists { get; set; }
        public virtual DbSet<ImportCreditPayment> ImportCreditPayments { get; set; }
        public virtual DbSet<ImportReturn> ImportReturns { get; set; }
        public virtual DbSet<ImportTransaction> ImportTransactions { get; set; }
        public virtual DbSet<ImpReturnTran> ImpReturnTrans { get; set; }
        public virtual DbSet<Indent> Indents { get; set; }
        public virtual DbSet<IndentItem> IndentItems { get; set; }
        public virtual DbSet<IndentMRR> IndentMRRs { get; set; }
        public virtual DbSet<IndMRRItem> IndMRRItems { get; set; }
        public virtual DbSet<InventoryDailyDataSet> InventoryDailyDataSets { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
        public virtual DbSet<LettersCount> LettersCounts { get; set; }
        public virtual DbSet<MasterProduct> MasterProducts { get; set; }
        public virtual DbSet<MasterProductCategory> MasterProductCategories { get; set; }
        public virtual DbSet<MasterProductSubCategory> MasterProductSubCategories { get; set; }
        public virtual DbSet<MiscFuntion> MiscFuntions { get; set; }
        public virtual DbSet<MultipleHoliday> MultipleHolidays { get; set; }
        public virtual DbSet<NameTitle> NameTitles { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<OfferItem> OfferItems { get; set; }
        public virtual DbSet<OrderPayment> OrderPayments { get; set; }
        public virtual DbSet<PaymentBody> PaymentBodies { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<PaySheet> PaySheets { get; set; }
        public virtual DbSet<PosOrder> PosOrders { get; set; }
        public virtual DbSet<PosOrderTransaction> PosOrderTransactions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductDistribute> ProductDistributes { get; set; }
        public virtual DbSet<ProductSubCategory> ProductSubCategories { get; set; }
        public virtual DbSet<ProductType_Table> ProductType_Table { get; set; }
        public virtual DbSet<ProSalePrice> ProSalePrices { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }
        public virtual DbSet<Refund> Refunds { get; set; }
        public virtual DbSet<RefundStatusType> RefundStatusTypes { get; set; }
        public virtual DbSet<RefundTransaction> RefundTransactions { get; set; }
        public virtual DbSet<ReligionList> ReligionLists { get; set; }
        public virtual DbSet<Restriction> Restrictions { get; set; }
        public virtual DbSet<Salary> Salaries { get; set; }
        public virtual DbSet<SalaryTryToPay> SalaryTryToPays { get; set; }
        public virtual DbSet<SaleServiceChangeArchive> SaleServiceChangeArchives { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleItem> ScheduleItems { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<ServicesCondition> ServicesConditions { get; set; }
        public virtual DbSet<ServicesCustomer> ServicesCustomers { get; set; }
        public virtual DbSet<ServicesProduct> ServicesProducts { get; set; }
        public virtual DbSet<ServicesService> ServicesServices { get; set; }
        public virtual DbSet<ServicesSupplimentary> ServicesSupplimentaries { get; set; }
        public virtual DbSet<ServiceStatu> ServiceStatus { get; set; }
        public virtual DbSet<ServiceSubCategory> ServiceSubCategories { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<StockDailyReturn> StockDailyReturns { get; set; }
        public virtual DbSet<StockDailySold> StockDailySolds { get; set; }
        public virtual DbSet<StockDailyTopReturn> StockDailyTopReturns { get; set; }
        public virtual DbSet<StockDailyTopSold> StockDailyTopSolds { get; set; }
        public virtual DbSet<StockImport> StockImports { get; set; }
        public virtual DbSet<StockWarehouse> StockWarehouses { get; set; }
        public virtual DbSet<StockWaste> StockWastes { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Supplementary> Supplementaries { get; set; }
        public virtual DbSet<SupplementaryCategory> SupplementaryCategories { get; set; }
        public virtual DbSet<SupplementarySubCategory> SupplementarySubCategories { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierDebit> SupplierDebits { get; set; }
        public virtual DbSet<SupplierDebitPayment> SupplierDebitPayments { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TagItem> TagItems { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserInformation> UserInformations { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<Vat> Vats { get; set; }
        public virtual DbSet<Waste> Wastes { get; set; }
        public virtual DbSet<WasteProduct> WasteProducts { get; set; }
        public virtual DbSet<WasteTransaction> WasteTransactions { get; set; }
        public virtual DbSet<WasteType> WasteTypes { get; set; }
        public virtual DbSet<WImport> WImports { get; set; }
        public virtual DbSet<WImpTran> WImpTrans { get; set; }
        public virtual DbSet<WorkingDay> WorkingDays { get; set; }
        public virtual DbSet<WorkingSchedule> WorkingSchedules { get; set; }
        public virtual DbSet<WorkOrder> WorkOrders { get; set; }
        public virtual DbSet<WorkOrderItem> WorkOrderItems { get; set; }
        public virtual DbSet<ViewAccNameAssignWithUser> ViewAccNameAssignWithUsers { get; set; }
        public virtual DbSet<ViewAccount> ViewAccounts { get; set; }
        public virtual DbSet<ViewAccountName> ViewAccountNames { get; set; }
        public virtual DbSet<ViewAfterSaleService> ViewAfterSaleServices { get; set; }
        public virtual DbSet<ViewAmountCoupon> ViewAmountCoupons { get; set; }
        public virtual DbSet<ViewAssignedMethodName> ViewAssignedMethodNames { get; set; }
        public virtual DbSet<ViewBorrowProduct> ViewBorrowProducts { get; set; }
        public virtual DbSet<ViewCategory> ViewCategories { get; set; }
        public virtual DbSet<ViewCreditCustomer> ViewCreditCustomers { get; set; }
        public virtual DbSet<ViewCreditLimit> ViewCreditLimits { get; set; }
        public virtual DbSet<ViewCustomer> ViewCustomers { get; set; }
        public virtual DbSet<ViewDailyAttendance> ViewDailyAttendances { get; set; }
        public virtual DbSet<ViewDeliveryCharge> ViewDeliveryCharges { get; set; }
        public virtual DbSet<ViewDeliveryChargeCoupon> ViewDeliveryChargeCoupons { get; set; }
        public virtual DbSet<ViewDiscountItem> ViewDiscountItems { get; set; }
        public virtual DbSet<ViewEmpSalary> ViewEmpSalaries { get; set; }
        public virtual DbSet<ViewEvent> ViewEvents { get; set; }
        public virtual DbSet<ViewHoliday> ViewHolidays { get; set; }
        public virtual DbSet<ViewHolidayPackage> ViewHolidayPackages { get; set; }
        public virtual DbSet<ViewImportCreditPayment> ViewImportCreditPayments { get; set; }
        public virtual DbSet<ViewImportTransaction> ViewImportTransactions { get; set; }
        public virtual DbSet<ViewIndent> ViewIndents { get; set; }
        public virtual DbSet<ViewIndentItem> ViewIndentItems { get; set; }
        public virtual DbSet<ViewIndentMRR> ViewIndentMRRs { get; set; }
        public virtual DbSet<ViewIndentMrrItem> ViewIndentMrrItems { get; set; }
        public virtual DbSet<ViewLeaveType> ViewLeaveTypes { get; set; }
        public virtual DbSet<ViewMainProduct> ViewMainProducts { get; set; }
        public virtual DbSet<ViewMasterProduct> ViewMasterProducts { get; set; }
        public virtual DbSet<ViewMasterProductCategory> ViewMasterProductCategories { get; set; }
        public virtual DbSet<ViewOffer> ViewOffers { get; set; }
        public virtual DbSet<ViewOfferItem> ViewOfferItems { get; set; }
        public virtual DbSet<ViewOrderTransaction> ViewOrderTransactions { get; set; }
        public virtual DbSet<ViewPayment> ViewPayments { get; set; }
        public virtual DbSet<ViewPaymentMethod> ViewPaymentMethods { get; set; }
        public virtual DbSet<ViewPaymentType> ViewPaymentTypes { get; set; }
        public virtual DbSet<ViewPaySheet> ViewPaySheets { get; set; }
        public virtual DbSet<ViewPosOrder> ViewPosOrders { get; set; }
        public virtual DbSet<ViewPosRefund> ViewPosRefunds { get; set; }
        public virtual DbSet<ViewProductCategory> ViewProductCategories { get; set; }
        public virtual DbSet<ViewProductDistribute> ViewProductDistributes { get; set; }
        public virtual DbSet<ViewProduct> ViewProducts { get; set; }
        public virtual DbSet<ViewPurchaseOrder> ViewPurchaseOrders { get; set; }
        public virtual DbSet<ViewPurchaseOrderTransaction> ViewPurchaseOrderTransactions { get; set; }
        public virtual DbSet<ViewRestriction> ViewRestrictions { get; set; }
        public virtual DbSet<ViewSchedule> ViewSchedules { get; set; }
        public virtual DbSet<ViewService> ViewServices { get; set; }
        public virtual DbSet<ViewServicesCondition> ViewServicesConditions { get; set; }
        public virtual DbSet<ViewServicesProduct> ViewServicesProducts { get; set; }
        public virtual DbSet<ViewServicesService> ViewServicesServices { get; set; }
        public virtual DbSet<ViewServicesSupplementary> ViewServicesSupplementaries { get; set; }
        public virtual DbSet<ViewStockDailySold> ViewStockDailySolds { get; set; }
        public virtual DbSet<ViewStockImport> ViewStockImports { get; set; }
        public virtual DbSet<ViewStockProduct> ViewStockProducts { get; set; }
        public virtual DbSet<ViewStockWarehouse> ViewStockWarehouses { get; set; }
        public virtual DbSet<ViewStockWaste> ViewStockWastes { get; set; }
        public virtual DbSet<ViewSubCategory> ViewSubCategories { get; set; }
        public virtual DbSet<ViewSupplementary> ViewSupplementaries { get; set; }
        public virtual DbSet<ViewSupplier> ViewSuppliers { get; set; }
        public virtual DbSet<ViewTag> ViewTags { get; set; }
        public virtual DbSet<ViewTagItem> ViewTagItems { get; set; }
        public virtual DbSet<ViewUserAttInput> ViewUserAttInputs { get; set; }
        public virtual DbSet<ViewUserList> ViewUserLists { get; set; }
        public virtual DbSet<ViewVat> ViewVats { get; set; }
        public virtual DbSet<ViewWasteProduct> ViewWasteProducts { get; set; }
        public virtual DbSet<ViewWasteTransaction> ViewWasteTransactions { get; set; }
        public virtual DbSet<ViewWasteType> ViewWasteTypes { get; set; }
        public virtual DbSet<ViewWImport> ViewWImports { get; set; }
        public virtual DbSet<ViewWorkingSchedule> ViewWorkingSchedules { get; set; }
        public virtual DbSet<ViewWorkOrder> ViewWorkOrders { get; set; }
        public virtual DbSet<ViewWorkOrderItem> ViewWorkOrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountName>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<AccType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Color>()
                .Property(e => e.ColorName)
                .IsUnicode(false);

            modelBuilder.Entity<CountryList>()
                .Property(e => e.CountryName)
                .IsUnicode(false);

            modelBuilder.Entity<CountryList>()
                .Property(e => e.CountryCode)
                .IsUnicode(false);

            modelBuilder.Entity<CountryList>()
                .Property(e => e.DialingCode)
                .IsUnicode(false);

            modelBuilder.Entity<CountryList>()
                .Property(e => e.Nationality)
                .IsUnicode(false);

            modelBuilder.Entity<DivisionList>()
                .Property(e => e.DivisionName)
                .IsUnicode(false);

            modelBuilder.Entity<Holiday>()
                .Property(e => e.HolidayName)
                .IsUnicode(false);

            modelBuilder.Entity<Holiday>()
                .Property(e => e.MonthName)
                .IsUnicode(false);

            modelBuilder.Entity<Holiday>()
                .Property(e => e.Year)
                .IsUnicode(false);

            modelBuilder.Entity<HolidayPackageList>()
                .Property(e => e.HolidayPackName)
                .IsUnicode(false);

            modelBuilder.Entity<LettersCount>()
                .Property(e => e.Letters)
                .IsUnicode(false);

            modelBuilder.Entity<MultipleHoliday>()
                .Property(e => e.MonthName)
                .IsUnicode(false);

            modelBuilder.Entity<NameTitle>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<OfferItem>()
                .Property(e => e.ProductName)
                .IsUnicode(false);

            modelBuilder.Entity<PosOrderTransaction>()
                .Property(e => e.ServiceName)
                .IsUnicode(false);

            modelBuilder.Entity<RefundStatusType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ReligionList>()
                .Property(e => e.Religion)
                .IsUnicode(false);

            modelBuilder.Entity<SaleServiceChangeArchive>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<ServicesCustomer>()
                .Property(e => e.Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<ServicesCustomer>()
                .Property(e => e.AlternateMobile)
                .IsUnicode(false);

            modelBuilder.Entity<ServiceStatu>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<ServiceType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Size>()
                .Property(e => e.SizeName)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.EmpId)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.CardNumber)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.Picture)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PictureOriginalName)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.Nationality)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.NationalId)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.NationalIdBackImg)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.NationalIdFontImg)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.MobileNo)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.ParAddress)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.ParAddressLine1)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.ParCountry)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.ParState)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.ParCity)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.ParArea)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.ParPotalCode)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PreAddress)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PreAddressLine1)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PreCountry)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PreState)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PreCity)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PreArea)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.PrePostalCode)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.x1)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.y1)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.x2)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.y2)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.width)
                .IsUnicode(false);

            modelBuilder.Entity<UserInformation>()
                .Property(e => e.height)
                .IsUnicode(false);

            modelBuilder.Entity<Waste>()
                .Property(e => e.ReffNo)
                .IsUnicode(false);

            modelBuilder.Entity<WasteProduct>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<WorkingDay>()
                .Property(e => e.Day)
                .IsUnicode(false);

            modelBuilder.Entity<WorkingSchedule>()
                .Property(e => e.ScheduleName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewAccountName>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<ViewAccountName>()
                .Property(e => e.AccTypeName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewDailyAttendance>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewDailyAttendance>()
                .Property(e => e.Picture)
                .IsUnicode(false);

            modelBuilder.Entity<ViewEmpSalary>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<ViewEmpSalary>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewEmpSalary>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewEmpSalary>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewEmpSalary>()
                .Property(e => e.Picture)
                .IsUnicode(false);

            modelBuilder.Entity<ViewHoliday>()
                .Property(e => e.HolidayName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewHoliday>()
                .Property(e => e.MonthName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewHoliday>()
                .Property(e => e.Year)
                .IsUnicode(false);

            modelBuilder.Entity<ViewHolidayPackage>()
                .Property(e => e.HolidayPackName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewOrderTransaction>()
                .Property(e => e.ServiceName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewPaySheet>()
                .Property(e => e.AssignUserName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewProductDistribute>()
                .Property(e => e.ColorName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewProduct>()
                .Property(e => e.ServiceName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserAttInput>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserAttInput>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserAttInput>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserAttInput>()
                .Property(e => e.Picture)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.Religion)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.Nationalitiy)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.Picture)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PictureOriginalName)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.NationalId)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.NationalIdBackImg)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.NationalIdFontImg)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.MobileNo)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParAddress)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParAddressLine1)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParCountry)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParDivision)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParState)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParCity)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParArea)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.ParPotalCode)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PreAddress)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PreAddressLine1)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PreCountry)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PreDivision)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PreState)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PreCity)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PreArea)
                .IsUnicode(false);

            modelBuilder.Entity<ViewUserList>()
                .Property(e => e.PrePostalCode)
                .IsUnicode(false);

            modelBuilder.Entity<ViewWasteProduct>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<ViewWasteProduct>()
                .Property(e => e.WasteReffNo)
                .IsUnicode(false);

            modelBuilder.Entity<ViewWorkingSchedule>()
                .Property(e => e.ScheduleName)
                .IsUnicode(false);
        }
    }
}
