using System;
using System.Collections.Generic;
using NUnit.Framework;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    public static class LocationTests {
        
        [Test]
        public static void Tests() {
            var locations = new[] {
                new { location = new Location(12, 23), toString = "12,23" },
                new { location = new Location(-4.3, 0.20), toString = "-4.3,0.2" },
            };

            foreach (var l in locations) {
                var x = l;
                Assert.AreEqual(x.toString, x.location.ToString());

                
                var parsedLocation = LocationFieldParser.Parse(x.toString);
                Assert.AreEqual(x.location, parsedLocation);
                Assert.AreEqual(x.location.Latitude, parsedLocation.Latitude);
                Assert.AreEqual(x.location.Longitude, parsedLocation.Longitude);
                
            }

            var invalidLatitudes = new[] {-100, 120};
            foreach (var x in invalidLatitudes) {
                var latitude = x;

                Assert.IsFalse(Location.IsValidLatitude(latitude));

                Assert.Throws<ArgumentOutOfRangeException>(() => new Location(latitude, 0));
            }

            var invalidLongitudes = new[] {-200, 999};
            foreach (var x in invalidLongitudes) {
                var longitude = x;

                Assert.IsFalse(Location.IsValidLongitude(longitude), "Valid longitude");

                Assert.Throws<ArgumentOutOfRangeException>(() => new Location(0, longitude), "Invalid longitude throws");
            }

            
            foreach (var lat in invalidLatitudes)
                foreach (var lng in invalidLongitudes) {
                    var loc = Location.TryCreate(lat, lng);
                    Assert.IsNull(loc);
                }
            

            
                foreach (var l in locations) {
                    var loc = l.location;
                    var loc2 = Location.TryCreate(loc.Latitude, loc.Longitude);
                    Assert.IsNotNull(loc2);
                    Assert.AreEqual(loc, loc2);
                }
            
        }
    }
}
