const axios = require('axios');
const cheerio = require('cheerio');
const iconv = require('iconv-lite');
const fs = require('fs');

const url = 'https://www.sangji.ac.kr/prog/carteGuidance/kor/sub07_10_03/CS/calendar.do';

async function Get_HTML() {
    try {
        const response = await axios({
            method: 'get',
            url,
            responseType: 'arraybuffer',
            responseEncoding: 'binary',
        });

        // Decode the response using EUC-KR encoding
        const html = iconv.decode(response.data, 'EUC-KR');
        return html;
    } catch (error) {
        console.error('Error fetching HTML:', error);
        throw error;
    }
}

function Extract(html) {
    const $ = cheerio.load(html);

    // Attempt to find an element similar to #coltable
    const possibleTables = $('table');

    if (!possibleTables.length) {
        console.error('No table elements found.');
        return '';
    }

    console.log(`Found ${possibleTables.length} table elements.`);

    // Log IDs and classes of found tables
    possibleTables.each((i, table) => {
        const id = $(table).attr('id');
        const className = $(table).attr('class');
        console.log(`Table ${i + 1}: id=${id}, class=${className}`);
    });

    // Assuming #coltable is a table with a specific class or id
    const tableElement = $('table[id="coltable"], table.class("some-class")'); // Adjust selector based on inspection

    if (!tableElement.length) {
        console.error('#coltable or a similar table not found.');
        return '';
    }

    console.log("#coltable or a similar table found.");

    // Check if tbody exists within the found table
    const tbodyElement = tableElement.find('tbody');
    if (!tbodyElement.length) {
        console.error('tbody not found within the table.');
        return '';
    }

    console.log("tbody found within the table.");

    // Select the specific tr element
    const trElement = tbodyElement.find('tr').first(); // First tr within tbody

    if (!trElement.length) {
        console.error('No matching tr element found.');
        return '';
    }

    console.log("First tr element found within the table > tbody.");

    let texts = [];
    let attributes = [];

    // Extract text and attributes from each td
    trElement.find('td').each((i, tdElem) => {
        const tdElement = $(tdElem);
        let tdTexts = [];

        // Get rowspan and width attributes
        const rowspan = tdElement.attr('rowspan') || 'N/A';
        const width = tdElement.attr('width') || 'N/A';

        // Add the attributes to the attributes array
        attributes.push(`td ${i + 1} - rowspan: ${rowspan}, width: ${width}`);

        // Traverse through all child nodes of the td element
        tdElement.contents().each((i, child) => {
            if (child.type === 'text') {
                tdTexts.push($(child).text().trim());
            } else if (child.name === 'br') {
                tdTexts.push('\n');
            } else if (child.type === 'tag') {
                tdTexts.push($(child).text().trim());
            }
        });

        // Combine texts for the current td and add to texts array
        let combinedTexts = tdTexts.join('').replace(/\n+/g, '\n').split('\n').map(line => line.trim()).filter(line => line).join('\n');
        texts.push(combinedTexts);
    });

    // Join the texts from all tds into a single string
    const cleanedTexts = texts.join('\n\n'); // Adding extra newlines between tds for clarity

    // Combine texts and attributes for output
    const output = `${cleanedTexts}\n\nAttributes:\n${attributes.join('\n')}`;
    return output;
}

function SaveFile(filename, text) {
    try {
        fs.writeFileSync(filename, text, 'utf8');
        console.log(`Saved to ${filename}`);
    } catch (error) {
        console.error('Error saving file:', error);
        throw error;
    }
}

// Main function to orchestrate the scraping
async function Main() {
    try {
        const html = await Get_HTML();
        console.log('HTML fetched successfully.');
        const cleanedTexts = Extract(html);
        console.log('Text extraction completed.');
        SaveFile('창조관 식당 메뉴.txt', cleanedTexts);
    } catch (error) {
        console.error('An error occurred:', error);
    }
}

Main();