// node -v  # This will print the version of Node.js installed.
// npm -v   # This will print the version of npm installed.

// npm install fs puppeteer

const puppeteer = require('puppeteer');
const fs = require('fs');

async function scrapeMenu()
{
    const browser = await puppeteer.launch();
    const page = await browser.newPage();

    // Navigate to the page
    await page.goto('https://www.sangji.ac.kr/prog/carteGuidance/kor/sub07_10_03/CS/calendar.do');

    // Wait for the table to load
    await page.waitForSelector('#coltable > tbody');

    // Extract the days of the week and dates from the table headers
    const daysWithDates = await page.evaluate(() =>
    {
        const daysOfWeek = [];
        const headers = document.querySelectorAll('#coltable thead th div.rows.week');

        headers.forEach((header, index) =>
        {
            if (index > 0)
            { // Skip the first column ("비고")
                const dayName = header.querySelector('span').innerText; // e.g., MON
                const dateText = header.querySelector('em').innerText.replace(/\(|\)/g, ''); // e.g., 2024-09-30
                const fullDayName = `(${dayName} ${dateText})`; // Format as (MON 2024-09-30)
                daysOfWeek.push(fullDayName);
            }
        });

        return daysOfWeek;
    });

    // Scrape the table content
    const result = await page.evaluate((daysWithDates) =>
    {
        let output = '';

        // Select the table body
        const rows = document.querySelectorAll('#coltable > tbody > tr');

        // Iterate through rows and extract data
        rows.forEach((row) =>
        {
            const mealType = row.querySelector('th').innerText.trim();
            output += `\n====================\n${mealType}\n====================\n`;

            // Select and iterate over each cell in the row
            const cells = row.querySelectorAll('td');
            cells.forEach((cell, index) =>
            {
                let menuText = cell.innerHTML
                    .replace(/<br>/g, '\n') // Replace <br> with newline
                    .replace(/<div class="obj">|<\/div>/g, '') // Remove <div> tags
                    .replace(/&amp;/g, '&') // Decode &amp;
                    .trim();

                // If menuText is not empty, append it with the day of the week and date
                if (menuText)
                {
                    output += `${daysWithDates[index]}:\n${menuText}\n\n`;
                }
            });
        });

        return output;
    }, daysWithDates);

    // Write the formatted result to a text file
    fs.writeFileSync('창조관_식당_메뉴.txt', result);

    // Get the current date and time
    const currentDateTime = new Date();
    console.log(`저장 완료 => ${currentDateTime}`);

    // Close the browser
    await browser.close();
}

// Execute the scraping function
scrapeMenu();