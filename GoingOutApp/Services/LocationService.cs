﻿using GoingOutApp.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using GoingOutApp.Models;
using System.Globalization;

namespace GoingOutApp.Services
{
    public static class LocationService
    {
        public static async Task<Location> GetLocationByCords(string location)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"http://dev.virtualearth.net/REST/v1/Locations?query={location}&key=tdR8B4UFCok6HiAPmoQ3~K8lYPO2jpRrn2Eo7sfgHRQ~ArKu6p1ZhDGu_ekMQ6eam5QBW67AVHme_OOL_4LkpzH0P8ScgJT2w-UtzHnjRbr4";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    JObject jObject = JObject.Parse(json);

                    var geocodePoints = jObject.SelectTokens("$.resourceSets[0].resources[0].geocodePoints[*].coordinates");

                    List<Location> points = new List<Location>();

                    foreach (var coordinates in geocodePoints)
                    {
                        double latitude = Convert.ToDouble(coordinates[0]);
                        double longitude = Convert.ToDouble(coordinates[1]);

                        points.Add(new Location(latitude, longitude));
                    }
                    return points[0];
                }
                else
                {
                    Console.WriteLine($"Błąd: {response.StatusCode}");
                    return new Location(52.2319581, 21.0067249);
                }
            }
        }
        public static async Task<AddressInfo> GetAddressInfoByCoords(double latitude, double longitude)
        {
            using (HttpClient client = new HttpClient())
            {
                CultureInfo culture = new CultureInfo("en-US");


                string apiRequest = $"https://api.geoapify.com/v1/geocode/reverse?lat={latitude.ToString(culture)}&lon={longitude.ToString(culture)}&format=json&apiKey=ed8c407bbbc1436b94f087ea02d60a77";
                HttpResponseMessage response = await client.GetAsync(apiRequest);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(json);

                    var address = jObject.SelectToken("$.results[0]");

                    string city = address?.Value<string>("city") ?? "";
                    string street = address?.Value<string>("street") ?? "";
                    string houseNumber = address?.Value<string>("housenumber") ?? "";
                    string postalCode = address?.Value<string>("postcode") ?? "";

                    return new AddressInfo(city, street, houseNumber, postalCode);
                }
                else
                {
                    Console.WriteLine($"Błąd: {response.StatusCode}");
                    return new AddressInfo("", "", "", "");
                }
            }
        }

        public static SolidColorBrush GetRandomBrush()
        {
            // Get the list of properties from the Brushes class using reflection
            var properties = typeof(Brushes).GetProperties();

            // Get a random index
            Random random = new Random();
            int randomIndex = random.Next(properties.Length);

            // Get the selected Brush property and return it
            var selectedBrush = properties[randomIndex].GetValue(null, null) as SolidColorBrush;
            return selectedBrush;
        }
    }
}