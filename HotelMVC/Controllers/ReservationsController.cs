﻿using HotelMVC.Extensions;
using HotelMVC.Models;
using HotelMVC.ViewModels;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace HotelMVC.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController()
        {
            _context = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new ReservationFormViewModel
            {
                Rooms = _context.Rooms.ToList()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservationFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Rooms = _context.Rooms.ToList();
                return View("Create", viewModel);
            }

            var price = 50.00;

            var reservation = new Reservations
            {
                CustomerId = User.Identity.GetUserId(),
                RoomId = viewModel.RoomId,
                Price = price,
                ArrivalDate = viewModel.GetArrivalDate(),
                DepartureDate = viewModel.GetDepartureDate()
            };

            //Check DB for conflicting reservation dates with the same room Id
            if (_context.Reservations.Any(
                r => r.RoomId == reservation.RoomId && 
                reservation.ArrivalDate.IsInRange(r.ArrivalDate, r.DepartureDate) || 
                reservation.DepartureDate.IsInRange(r.ArrivalDate, r.DepartureDate)))
                    return View("Create", viewModel);

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }


    }
}