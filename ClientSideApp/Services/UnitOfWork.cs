using MyModel.Models.DTOs;
using MyModel.Models.Entitties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IRepository<Client, ClientDTO>> _clientRepository;
        private readonly Lazy<IRepository<Ceremony, CeremonyDTO>> _ceremonyRepository;
        private readonly Lazy<IRepository<DeadPerson, DeadPersonDTO>> _deadPersonRepository;
        private readonly Lazy<IRepository<Order, OrderDTO>> _orderRepository;
        private readonly Lazy<IRepository<Product, ProductDTO>> _productRepository;
        private readonly Lazy<IRepository<ProductOrder, ProductOrderDTO>> _productOrderRepository;
        private readonly Lazy<IRepository<Report, ReportDTO>> _reportRepository;
        private readonly Lazy<IEmailService> _emailService;

        public UnitOfWork(ISettingsService settingsService)
        {
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(settingsService));
            _clientRepository = new Lazy<IRepository<Client, ClientDTO>>(() => new Repository<Client, ClientDTO>(settingsService));
            _ceremonyRepository = new Lazy<IRepository<Ceremony, CeremonyDTO>>(() => new Repository<Ceremony, CeremonyDTO>(settingsService));
            _deadPersonRepository = new Lazy<IRepository<DeadPerson, DeadPersonDTO>>(() => new Repository<DeadPerson, DeadPersonDTO>(settingsService));
            _orderRepository = new Lazy<IRepository<Order, OrderDTO>>(() => new OrderRepository(settingsService));
            _productRepository = new Lazy<IRepository<Product, ProductDTO>>(() => new Repository<Product, ProductDTO>(settingsService));
            _productOrderRepository = new Lazy<IRepository<ProductOrder, ProductOrderDTO>>(() => new Repository<ProductOrder, ProductOrderDTO>(settingsService));
            _reportRepository = new Lazy<IRepository<Report, ReportDTO>>(() => new Repository<Report, ReportDTO>(settingsService));
            _emailService = new Lazy<IEmailService>(() => new EmailService(settingsService));
        }
        public IUserRepository UserRepository => _userRepository.Value;

        public IRepository<Client, ClientDTO> ClientRepository => _clientRepository.Value;

        public IRepository<Ceremony, CeremonyDTO> CeremonyRepository => _ceremonyRepository.Value;

        public IRepository<DeadPerson, DeadPersonDTO> DeadPersonRepository => _deadPersonRepository.Value;

        public IRepository<Order, OrderDTO> OrderRepository => _orderRepository.Value;

        public IRepository<Product, ProductDTO> ProductRepository => _productRepository.Value;

        public IRepository<ProductOrder, ProductOrderDTO> ProductOrderRepository => _productOrderRepository.Value;

        public IRepository<Report, ReportDTO> ReportRepository => _reportRepository.Value;

        public IEmailService EmailService => _emailService.Value;
    }
}
