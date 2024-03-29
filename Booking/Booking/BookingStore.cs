﻿using Booking.Car;
using Booking.Customer;
using Booking.Data;
using Booking.Seed;

namespace Booking.Booking;

public class BookingStore
{
    private static BookingStore? _bookingStore;
    private IDataLayer<IBooking> _dataLayer;

    public BookingStore()
    {
        _dataLayer = new DataLayer<IBooking>();
    }
    

    public static BookingStore GetInstance()
    {
        return _bookingStore ??= new BookingStore();
    }

    public List<IBooking> GetByPeriod(DateTime from, DateTime to)
    {
        return _dataLayer.Entities.FindAll(booking => from <= booking.From && to >= booking.To);
    }

    public void Init(FakeData<IBooking> fakeBookings)
    {
        _dataLayer = fakeBookings;
    }


    public Bill? Add(ICar car, ICustomer customer, DateTime from, DateTime to, short forecastKilometer)
    {
        return _dataLayer.Add(new Booking(car, customer, from, to, forecastKilometer)).ForecastBill;
    }

    public ClosedBooking Close(IBooking bookingToClose, short actualKilometers)
    {
        ClosedBooking closedBooking = new ClosedBooking(bookingToClose, actualKilometers);
        _dataLayer.Update(closedBooking);
        return closedBooking;
    }

    public IBooking? GetOpenByRegistration(string registration)
    {
        return EntitiesWhereRegistration(registration)
            .FirstOrDefault(booking => booking.Status == Status.Open);
    }

    private IEnumerable<IBooking> EntitiesWhereRegistration(string registration)
    {
        return _dataLayer.Entities
            .Where(booking => booking.Car.Registration.ToString().Equals(registration));
    }

    public IBooking? GetByRegistration(string registration)
    {
        return EntitiesWhereRegistration(registration)
            .FirstOrDefault();
    }

    public IBooking Open(IBooking booking)
    {
        booking.Status = Status.Open;
        return _dataLayer.Update(booking);
    }

    public IBooking? GetPendingOrOpen()
    {
        return _dataLayer.Entities
            .Where(booking => booking.Status == Status.Open )
            .FirstOrDefault(booking => booking.Status == Status.Pending);
    }

    public IBooking? GetByCustomerByPeriod(AuthenticatedCustomer customer, DateTime @from, DateTime to)
    {
        return _dataLayer.Entities
            .Where(booking => booking.Customer.Equals(customer))
            .FirstOrDefault(booking => from <= booking.From || to >= booking.To);
    }

}