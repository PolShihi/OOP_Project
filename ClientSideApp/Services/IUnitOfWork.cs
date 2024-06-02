using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRepository<Client, ClientDTO> ClientRepository { get; }
        IRepository<Ceremony, CeremonyDTO> CeremonyRepository { get; }
        IRepository<DeadPerson, DeadPersonDTO> DeadPersonRepository { get; }
        IRepository<Order, OrderDTO> OrderRepository { get; }
        IRepository<Product, ProductDTO> ProductRepository { get; }
        IRepository<ProductOrder, ProductOrderDTO> ProductOrderRepository { get; }
        IRepository<Report, ReportDTO> ReportRepository { get; }
        IEmailService EmailService { get; }
    }
}
