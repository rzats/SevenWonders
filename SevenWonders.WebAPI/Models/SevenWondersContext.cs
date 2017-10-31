using SevenWonders.WebAPI.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SevenWonders.DAL.Context

{
    public class SevenWondersContext : DbContext
    {
        public SevenWondersContext() : base("SevenWondersContext")
        {
            //Configuration.ProxyCreationEnabled = false;
        }
        public virtual DbSet<Administrator> Administrators { get; set; }
        public virtual DbSet<Airplane> Airplanes { get; set; }
        public virtual DbSet<Airport> Airports { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Country> Coutries { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Equipment> Equipments { get; set; }
        public virtual DbSet<ExtraPrice> ExtraPrices { get; set; }
        public virtual DbSet<Facility> Facilities { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<FoodType> FoodTypes { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<HotelsPhoto> HotelsPhotos { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomsPhoto> RoomsPhotos { get; set; }
        public virtual DbSet<RoomType> RoomTypes { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Tour> Tours { get; set; }
        public virtual DbSet<TourState> TourStates { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }

}