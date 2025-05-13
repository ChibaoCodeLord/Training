
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Training.Data;
using Training.Models;
namespace Training.Service
{
    public class AccountServiceImpl : AccountService
    {
        private TrainingDbContext db;
        public AccountServiceImpl(TrainingDbContext db)
        {
            this.db = db;
        }

        public bool Create(Models.Infor account)
        {
            using var transaction = db.Database.BeginTransaction();
            if (!Regex.IsMatch(account.PhoneNumber, "^(0?)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$"))
            {
                throw new Exception("Số điện thoại phải đủ 10 số");
            }
                try
            {
                
                if (db.Students.Any(s => s.Email == account.Email))
                    throw new Exception("Email đã tồn tại");
                if (db.Accounts.Any(a => a.Username == account.Username))
                    throw new Exception("Username đã tồn tại");
                if (db.Students.Any(s => s.PhoneNumber == account.PhoneNumber))
                    throw new Exception("Số điện thoại đã tồn tại");

               
                var dbAccount = new Account
                {
                    AccountId = Guid.NewGuid().ToString("N").Substring(0, 5),
                    Username = account.Username,
                    Password = account.Password,
                    Role = account.Role ?? "Student"
                };
                db.Accounts.Add(dbAccount);

                
                var dbStudent = new Student
                {
                    StudentId = dbAccount.AccountId, 
                    FullName = account.FullName,
                    Email = account.Email,
                    PhoneNumber = account.PhoneNumber,
                    DateOfBirth = DateOnly.FromDateTime(
                    account.DateOfBirth ?? throw new Exception("Ngày sinh không được để trống")
                     )
                };
                db.Students.Add(dbStudent);

                db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Error: {dbEx.InnerException?.Message}");
                throw new Exception("Lỗi khi lưu dữ liệu: " + dbEx.InnerException?.Message);
            }
        }

    }
}
