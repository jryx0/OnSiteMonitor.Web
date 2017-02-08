using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WFM.JW.HB.Models;
using WFM.JW.HB.Services;


using TB.ComponentModel;


namespace WFM.JW.HB.Console
{
    class Program
    {
        static void Main(string[] args)
        {


            BaseTypeServices basetypeService = new BaseTypeServices();

            BaseType basetype = new Models.BaseType();
            basetype.CreateDate = DateTime.Now;
            basetype.BaseTypeID = 1;
            basetype.Comment = "test comment";
            basetype.Enable = true;


            Object o = new object();


            o = basetypeService;

            BaseTypeServices t = o.ConvertTo<BaseTypeServices>();

            int i = o.ConvertTo<int>();

            //IParametersSpecification spec = new CriteriaSpecification("Comment", Models.CriteriaOperator.Equal, basetype.Comment);

            //spec = spec.And(new CriteriaSpecification("BaseTypeID", Models.CriteriaOperator.Equal, basetype.BaseTypeID))
            //         .Or(new CriteriaSpecification("Enable", Models.CriteriaOperator.Equal, basetype.Enable))
            //         .And(new CriteriaSpecification("CreateDate", Models.CriteriaOperator.Equal, basetype.CreateDate));

            
            //System.Console.WriteLine(spec.GetSpec());


            //System.Console.WriteLine(spec.GetSpecValue());




           // System.Console.WriteLine("{0:d}", k.Value.CreateDate);
            
            System.Console.ReadKey();
        }


      
    }
}
