using JKApi.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ImportTaxManager
    {
        jkDatabaseEntities context = new jkDatabaseEntities();

        public List<GetTaxAddressForImportService_Result>  GetTxRateAddress()
        {
            return context.GetTaxAddressForImportService().ToList();
        }
        
        public List<Address> GetAllAddress()
        {
            return context.Addresses.ToList();
        }
        public void Add(TaxRate taxrate)
        {
            context.TaxRates.Add(taxrate);
            context.SaveChanges();
        }
        public TaxRate GetTaxById(int id)
        {
            return context.TaxRates.Where(t => t.TaxRateId == id).FirstOrDefault();
        }
        public TaxRate GetByAddressId(int addressId)
        {
            return context.TaxRates.Where(t => t.AddressId == addressId).FirstOrDefault();
        }
        public void UpdateTaxRate(TaxRate taxRate)
        {
            TaxRate tempTaxRate = GetTaxById(taxRate.TaxRateId);

            tempTaxRate.citySalesTax = taxRate.citySalesTax;
            tempTaxRate.cityUseTax = taxRate.cityUseTax;
            tempTaxRate.ContractTaxRate = taxRate.ContractTaxRate;
            tempTaxRate.countySalesTax = taxRate.countySalesTax;
            tempTaxRate.countyUseTax = taxRate.countyUseTax;
            tempTaxRate.districtSalesTax = taxRate.districtSalesTax;
            tempTaxRate.districtUseTax = taxRate.districtUseTax;
            tempTaxRate.LeaseTaxRate = taxRate.LeaseTaxRate;
            tempTaxRate.stateSalesTax = taxRate.stateSalesTax;
            tempTaxRate.stateUseTax = taxRate.stateUseTax;
            tempTaxRate.SupplyTaxRate = taxRate.SupplyTaxRate;
            tempTaxRate.taxSales = taxRate.taxSales;
            tempTaxRate.taxUse = taxRate.taxUse;

            context.SaveChanges();
        }
    }
}
