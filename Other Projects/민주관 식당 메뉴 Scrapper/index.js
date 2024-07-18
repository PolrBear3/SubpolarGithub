const axios = require('axios');
const cheerio = require('cheerio');
const iconv = require('iconv-lite');
const fs = require('fs');
const path = require('path');

const url = 'http://sjcoop.or.kr/board/board.php?cmd=view&b_id=menu_1&num=301';

async function Get_HTML() {
    const response = await axios({
        method: 'get',
        url,
        responseType: 'arraybuffer',
        responseEncoding: 'binary',
    });

    // Decode the response using the appropriate encoding
    const html = iconv.decode(response.data, 'EUC-KR');
    return html;
}

Get_HTML().then((res) => {
    const $ = cheerio.load(res);

    // Get texts from <td>
    const tdElement = $('td[rowspan="4"][width="172"]').first();

    // Extract and log the text content from the nested elements
    const texts = [];
    tdElement.find('div').each((i, elem) => {
        $(elem).find('strong, br').each((j, nestedElem) => {
            if ($(nestedElem).is('strong')) {
                texts.push($(nestedElem).text().trim());
            }
            if ($(nestedElem).is('br')) {
                texts.push('\n');
            }
        });
    });

    // Combine texts
    const cleanedTexts = texts.join(' ').replace(/\n+/g, '\n').split('\n').map(line => line.trim()).filter(line => line).join('\n');
    
    // Save text tile to path
    fs.writeFileSync('민주관 학생식당 메뉴.txt', cleanedTexts, 'utf8');
    console.log(`저장 완료`);
})