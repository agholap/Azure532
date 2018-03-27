using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBot.Models
{
    public enum BedSizeOptions
    {
        King,
        Queen,
        Single,
        Double
    }

    public enum AmenitiesOptions
    {
        Kitechen,
        ExtraTowels,
        GymAccess,
        Wifi
    }

    [Serializable]
    public class RoomReservation
    {
        public BedSizeOptions? BedSize;
        public int? NumberOfOccupants;
        public DateTime? CheckInDate;
        public List<AmenitiesOptions> Amenities;

        public static IForm<RoomReservation> BuildForm()
        {
            return new FormBuilder<RoomReservation>()
                .Message("Welcome to help reservation bot!")
                .Build();
        }


    }
}