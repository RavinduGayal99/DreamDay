using DreamDay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamDay.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<User>>();
            var roleManager = service.GetService<RoleManager<IdentityRole<int>>>();
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
                await roleManager.CreateAsync(new IdentityRole<int>("Couple"));
                await roleManager.CreateAsync(new IdentityRole<int>("WeddingPlanner"));
            }

            var user = new User { UserName = "admin@dreamday.com", Email = "admin@dreamday.com", Name = "Admin User", EmailConfirmed = true };
            if (await userManager.FindByEmailAsync(user.Email) == null)
            {
                await userManager.CreateAsync(user, "Admin@123");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        public static async Task SeedSampleDataAsync(IServiceProvider service)
        {
            var context = service.GetService<ApplicationDbContext>();
            var userManager = service.GetService<UserManager<User>>();

            if (await context.Weddings.AnyAsync()) return; // DB has been seeded

            // --- 1. Seed 10 Vendors ---
            var vendors = new List<Vendor>
            {
                new Vendor { Name = "Timeless Snaps", Category = "Photography", ContactInfo = "info@timelesssnaps.com", Description = "Capturing moments that last a lifetime." },
                new Vendor { Name = "Gourmet Catering Co.", Category = "Catering", ContactInfo = "bookings@gourmetco.com", Description = "Exquisite culinary experiences." },
                new Vendor { Name = "Bloom & Petal", Category = "Florist", ContactInfo = "orders@bloomandpetal.com", Description = "Creative and beautiful floral arrangements." },
                new Vendor { Name = "The Grand Hall", Category = "Venue", ContactInfo = "events@thegrandhall.com", Description = "A majestic space for your grand day." },
                new Vendor { Name = "DJ Beats", Category = "Entertainment", ContactInfo = "contact@djbeats.com", Description = "Keeping the dance floor full." },
                new Vendor { Name = "Sweet Tiers Bakery", Category = "Bakery", ContactInfo = "cakes@sweettiers.com", Description = "Custom wedding cakes and desserts." },
                new Vendor { Name = "Elegant Attire Boutique", Category = "Attire", ContactInfo = "stylist@elegantattire.com", Description = "Find the perfect dress and suits." },
                new Vendor { Name = "Prestige Cars", Category = "Transportation", ContactInfo = "ride@prestigecars.com", Description = "Arrive in style with our luxury fleet." },
                new Vendor { Name = "Paper & Co.", Category = "Invitations", ContactInfo = "design@paperco.com", Description = "Bespoke wedding stationery." },
                new Vendor { Name = "Lakeside Manor", Category = "Venue", ContactInfo = "lakeside@events.com", Description = "A serene and picturesque setting." }
            };
            await context.Vendors.AddRangeAsync(vendors);
            await context.SaveChangesAsync();

            // --- 2. Seed Couple & Planner Users ---
            var coupleUser = new User { UserName = "couple@test.com", Email = "couple@test.com", Name = "Alex & Jordan", EmailConfirmed = true };
            await userManager.CreateAsync(coupleUser, "Couple@123");
            await userManager.AddToRoleAsync(coupleUser, "Couple");
            var coupleProfile = new Couple { UserId = coupleUser.Id };
            context.Couples.Add(coupleProfile);
            await context.SaveChangesAsync();

            var plannerUser = new User { UserName = "planner@test.com", Email = "planner@test.com", Name = "Jane Doe", EmailConfirmed = true };
            await userManager.CreateAsync(plannerUser, "Planner@123");
            await userManager.AddToRoleAsync(plannerUser, "WeddingPlanner");
            var plannerProfile = new WeddingPlanner { UserId = plannerUser.Id, BusinessName = "Dream Day Planners Inc." };
            context.WeddingPlanners.Add(plannerProfile);
            await context.SaveChangesAsync();

            // --- 3. Seed the Wedding ---
            var wedding = new Wedding
            {
                Title = "Alex & Jordan's Summer Wedding",
                WeddingDate = DateTime.Today.AddDays(120),
                Budget = 35000,
                CoupleId = coupleProfile.Id,
                WeddingPlannerId = plannerProfile.Id
            };
            context.Weddings.Add(wedding);
            await context.SaveChangesAsync();

            // --- 4. Seed Rich Data for the Wedding ---
            // Budget with multiple categories and expenses
            var budget = new Budget { WeddingId = wedding.Id };
            context.Budgets.Add(budget);
            await context.SaveChangesAsync();
            var budgetCategories = new List<BudgetCategory>
            {
                new BudgetCategory { BudgetId = budget.Id, Name = "Venue & Rentals", AllocatedAmount = 12000 },
                new BudgetCategory { BudgetId = budget.Id, Name = "Catering & Bar", AllocatedAmount = 10000 },
                new BudgetCategory { BudgetId = budget.Id, Name = "Photography & Video", AllocatedAmount = 5000 },
                new BudgetCategory { BudgetId = budget.Id, Name = "Attire & Rings", AllocatedAmount = 4000 },
                new BudgetCategory { BudgetId = budget.Id, Name = "Flowers & Decor", AllocatedAmount = 3000 },
                new BudgetCategory { BudgetId = budget.Id, Name = "Entertainment", AllocatedAmount = 1000 }
            };
            await context.BudgetCategories.AddRangeAsync(budgetCategories);
            await context.SaveChangesAsync();
            await context.Expenses.AddRangeAsync(
                new Expense { BudgetCategoryId = budgetCategories[0].Id, Description = "Venue Deposit", Amount = 6000 },
                new Expense { BudgetCategoryId = budgetCategories[1].Id, Description = "Catering Deposit", Amount = 5000 },
                new Expense { BudgetCategoryId = budgetCategories[2].Id, Description = "Photographer Retainer", Amount = 2500 }
            );

            // 10+ Checklist Tasks
            await context.Tasks.AddRangeAsync(
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Finalize Wedding Budget", DueDate = DateTime.Today.AddDays(-20), IsCompleted = true, Category = TaskCategory.OTHER },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Book Wedding Venue", DueDate = DateTime.Today.AddDays(-10), IsCompleted = true, Category = TaskCategory.VENUE },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Book Photographer", DueDate = DateTime.Today.AddDays(-9), IsCompleted = true, Category = TaskCategory.PHOTOGRAPHY },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Finalize Guest List", DueDate = DateTime.Today.AddDays(30), IsCompleted = false, Category = TaskCategory.OTHER },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Send out Invitations", DueDate = DateTime.Today.AddDays(60), IsCompleted = false, Category = TaskCategory.INVITATION },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Catering Tasting Session", DueDate = DateTime.Today.AddDays(45), IsCompleted = false, Category = TaskCategory.CATERING },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Choose Wedding Attire", DueDate = DateTime.Today.AddDays(50), IsCompleted = false, Category = TaskCategory.ATTIRE },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Book Entertainment (DJ/Band)", DueDate = DateTime.Today.AddDays(25), IsCompleted = false, Category = TaskCategory.OTHER },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Arrange Transportation", DueDate = DateTime.Today.AddDays(70), IsCompleted = false, Category = TaskCategory.OTHER },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Purchase Wedding Rings", DueDate = DateTime.Today.AddDays(55), IsCompleted = false, Category = TaskCategory.ATTIRE },
                new Models.ProjectTask { WeddingId = wedding.Id, Description = "Final Venue Walkthrough", DueDate = DateTime.Today.AddDays(110), IsCompleted = false, Category = TaskCategory.VENUE }
            );

            // 10+ Guests
            await context.Guests.AddRangeAsync(
                new Guest { WeddingId = wedding.Id, Name = "Michael Scott", RsvpStatus = GuestStatus.ATTENDING, MealPreference = "Chicken" },
                new Guest { WeddingId = wedding.Id, Name = "Pam Beesly", RsvpStatus = GuestStatus.ATTENDING, MealPreference = "Vegetarian" },
                new Guest { WeddingId = wedding.Id, Name = "Jim Halpert", RsvpStatus = GuestStatus.ATTENDING, MealPreference = "Beef" },
                new Guest { WeddingId = wedding.Id, Name = "Dwight Schrute", RsvpStatus = GuestStatus.PENDING, MealPreference = "Beets" },
                new Guest { WeddingId = wedding.Id, Name = "Angela Martin", RsvpStatus = GuestStatus.DECLINED },
                new Guest { WeddingId = wedding.Id, Name = "Kevin Malone", RsvpStatus = GuestStatus.ATTENDING, MealPreference = "Everything" },
                new Guest { WeddingId = wedding.Id, Name = "Stanley Hudson", RsvpStatus = GuestStatus.PENDING },
                new Guest { WeddingId = wedding.Id, Name = "Phyllis Vance", RsvpStatus = GuestStatus.ATTENDING, MealPreference = "Chicken" },
                new Guest { WeddingId = wedding.Id, Name = "Oscar Martinez", RsvpStatus = GuestStatus.ATTENDING, MealPreference = "Vegetarian" },
                new Guest { WeddingId = wedding.Id, Name = "Andy Bernard", RsvpStatus = GuestStatus.INVITED },
                new Guest { WeddingId = wedding.Id, Name = "Erin Hannon", RsvpStatus = GuestStatus.ATTENDING, MealPreference = "Beef" }
            );

            // Timeline, Tables, Messages, Reviews, etc.
            var timeline = new Timeline { WeddingId = wedding.Id };
            context.Timelines.Add(timeline);
            await context.SaveChangesAsync();
            await context.TimelineEvents.AddRangeAsync(
                new TimelineEvent { TimelineId = timeline.Id, Title = "Guests Arrive", StartTime = wedding.WeddingDate.AddHours(14) },
                new TimelineEvent { TimelineId = timeline.Id, Title = "Ceremony Begins", StartTime = wedding.WeddingDate.AddHours(14).AddMinutes(30) },
                new TimelineEvent { TimelineId = timeline.Id, Title = "Cocktail Hour", StartTime = wedding.WeddingDate.AddHours(15).AddMinutes(30) },
                new TimelineEvent { TimelineId = timeline.Id, Title = "Dinner Service", StartTime = wedding.WeddingDate.AddHours(17) }
            );
            await context.Tables.AddRangeAsync(
                new Table { WeddingId = wedding.Id, Name = "Head Table", Capacity = 10 },
                new Table { WeddingId = wedding.Id, Name = "Family Table 1", Capacity = 8 },
                new Table { WeddingId = wedding.Id, Name = "Friends Table 1", Capacity = 8 }
            );
            await context.Messages.AddRangeAsync(
                new Message { SenderId = coupleUser.Id, ReceiverId = plannerUser.Id, Content = "Hi Jane, have you had a chance to look at the catering options?" },
                new Message { SenderId = plannerUser.Id, ReceiverId = coupleUser.Id, Content = "Hi Alex & Jordan! Yes, I've sent over the top three proposals. Let me know your thoughts!" }
            );
            context.Reviews.Add(new Review { VendorId = vendors[0].Id, CoupleId = coupleProfile.Id, Rating = 5, Comment = "So professional and easy to work with during our engagement shoot!" });
            context.ActivityLogs.Add(new ActivityLog { ActionDescription = $"User '{coupleUser.Email}' registered.", UserId = coupleUser.Id });

            // --- Final Save ---
            await context.SaveChangesAsync();
        }
    }
}