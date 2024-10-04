const axios = require('axios');
const cheerio = require('cheerio');
const iconv = require('iconv-lite');
const fs = require('fs');

const url = 'http://sjcoop.or.kr/board/board.php?cmd=view&b_id=menu_3&num=557';

async function Get_HTML()
{
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

Get_HTML().then((res) =>
{
    const $ = cheerio.load(res);

    // Get the <td> element
    const tdElement = $('td[height="500"][valign="top"]');

    // Extract all text within the <td> element, including nested elements
    let textContent = tdElement.text().trim();

    // Remove excessive empty lines and spaces
    textContent = textContent.replace(/(\s*\n\s*)+/g, '\n').trim();

    // Exclude everything starting from "상기 식단은 조기 품절 및 식당 사정시 변경 될 수 있습니다."
    const cutoffText = "상기 식단은 조기 품절 및 식당 사정시 변경 될 수 있습니다.";
    const cutoffIndex = textContent.indexOf(cutoffText);
    if (cutoffIndex !== -1)
    {
        textContent = textContent.substring(0, cutoffIndex).trim();
    }

    // Save cleaned-up text content to a file
    fs.writeFileSync('창조관 학생식당 메뉴.txt', textContent, 'utf8');
    console.log('저장 완료');
});