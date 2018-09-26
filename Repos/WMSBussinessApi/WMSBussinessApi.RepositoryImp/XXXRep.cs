using WMSBussinessApi.Repository;
using WMSBussinessApi.Model.XXX;
using Dapper;
using System;
using System.Linq;
using Schubert.Framework.Data;
using Microsoft.Extensions.Logging;

namespace WMSBussinessApi.RepositoryImp
{
    public class XXXRep : DapperRepository<Model.Employee>, IXXXRep
    {
        public XXXRep(DapperContext dapperContext, ILoggerFactory loggerFactory = null) : base(dapperContext, loggerFactory)
        {
        }

        public int MyOp(XC xc)
        {
            throw new NotImplementedException();
        }

        public int DbCeshi()
        {
            return this.GetReadingConnection().ExecuteScalar<int>("SELECT EmployeeId FROM erp_basedata.employee LIMIT 1;");
        }
    }



    /*
      public void 数据库访问示例()
      {
          //Insert
          string query1 = "INSERT INTO Book(Name) VALUES(@name)";
          this.GetReadingConnection().Execute(query1, new { name = "C#本质论" });

          //Insert
          string query11 = "INSERT INTO Book(Name) VALUES(@name)";
          Book book = new Book();
          book.Name = "C#本质论";
          this.GetReadingConnection().ExecuteScalar.Execute(query11, book);

          //update
          string query2 = "UPDATE Book SET  Name=@name WHERE id =@id";
          this.GetReadingConnection().ExecuteScalar.Execute(query2, new { name = "C#本质论", id = 111 });

          //delete
          string query3 = "DELETE FROM Book WHERE id = @id";
          this.GetReadingConnection().ExecuteScalar.Execute(query3, new { id = 1111 });

          //query
          string query4 = "SELECT * FROM Book WHERE id = @id";
          this.GetReadingConnection().ExecuteScalar.Query<Book>(query4, new { id = 111 }).ToList();  //取一条 Connection.Query<Book>(query4, new { id = 111 }).SingleOrDefault();
      }

    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    */
}
