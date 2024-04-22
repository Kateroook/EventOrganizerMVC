using EventOrganizerDomain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EventOrganizerInfrastructure
{
    public class RolesInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            string moderatorRole = "Moderator";
            string participantRole = "Participant";
            string organizerRole = "Organizer";

            // Проверяем наличие ролей и создаем их, если они отсутствуют
            if (!await roleManager.RoleExistsAsync(moderatorRole))
            {
                await roleManager.CreateAsync(new Role { Name = moderatorRole });
            }

            if (!await roleManager.RoleExistsAsync(participantRole))
            {
                await roleManager.CreateAsync(new Role { Name = participantRole });
            }

            if (!await roleManager.RoleExistsAsync(organizerRole))
            {
                await roleManager.CreateAsync(new Role { Name = organizerRole });
            }

            string[] userEmails = {
                "participantseveryna.kate@gmail.com",
                "participantgosling@gmail.com",
                "participantshozhtam@gmail.com",
                "eventsorganizer@softserveinc.com",
                "eventsorganizer@epam.com",
                "eventsorganizer@agiliway.com",
                "phekorganizer@gmail.com",
                "knueventsorganizer@knu.ua.com",
                "ruparorganizer@gmail.com",
                "svitloorganizer@gmail.com",
                "magicorganizer@gmail.com",
                "promovuorganizer@gmail.com",
                "psheksorganizer@gmail.com"
};
            string[] roles = {
                "Moderator",
                "Participant",
                "Participant",
                "Organizer",
                "Organizer",
                "Organizer",
                "Organizer",
                "Organizer",
                "Organizer",
                "Organizer",
                "Organizer",
                "Organizer",
                "Organizer"
};

            string passwords = "Qwerty_11!";
            for (int i = 0; i < userEmails.Length; i++)
            {
                string userEmail = userEmails[i];
                string userPasswords = passwords;
                string role = roles[i];


                User user = new User { Email = userEmail, UserName = userEmail };
                IdentityResult result = await userManager.CreateAsync(user, userPasswords);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }

            }
        }
    }
}