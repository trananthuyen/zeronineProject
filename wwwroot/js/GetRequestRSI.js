export async function GetRSI15m() {
    try {
        const responsebtc = await fetch("/home/analysis?symbol=btcusdt&interval=15m");
        const responseeth = await fetch("/home/analysis?symbol=ethusdt&interval=15m");
        const responsebnb = await fetch("/home/analysis?symbol=bnbusdt&interval=15m");

        const btcdata = await responsebtc.json();
        const ethdata = await responseeth.json();
        const bnbdata = await responsebnb.json();

        const btcElement = document.getElementById("rsi-BTCUSDT-15m");
        const ethElement = document.getElementById("rsi-ETHUSDT-15m");
        const bnbElement = document.getElementById("rsi-BNBUSDT-15m");

        btcElement.textContent = btcdata;
        ethElement.textContent = ethdata;
        bnbElement.textContent = bnbdata;

    } catch (err) {
        console.error(err);
    }

    setTimeout(GetRSI15m, 5000);
}

export async function GetRSI1h() {
    try {
        const responsebtc = await fetch("/home/analysis?symbol=btcusdt&interval=1h");
        const responseeth = await fetch("/home/analysis?symbol=ethusdt&interval=1h");
        const responsebnb = await fetch("/home/analysis?symbol=bnbusdt&interval=1h");

        const btcdata = await responsebtc.json();
        const ethdata = await responseeth.json();
        const bnbdata = await responsebnb.json();

        const btcElement = document.getElementById("rsi-BTCUSDT-1h");
        const ethElement = document.getElementById("rsi-ETHUSDT-1h");
        const bnbElement = document.getElementById("rsi-BNBUSDT-1h");

        btcElement.textContent = btcdata;
        ethElement.textContent = ethdata;
        bnbElement.textContent = bnbdata;

    } catch (err) {
        console.error(err);
    }

    setTimeout(GetRSI1h, 5000);
}

export async function GetRSI4h() {
    try {
        const responsebtc = await fetch("/home/analysis?symbol=btcusdt&interval=4h");
        const responseeth = await fetch("/home/analysis?symbol=ethusdt&interval=4h");
        const responsebnb = await fetch("/home/analysis?symbol=bnbusdt&interval=4h");

        const btcdata = await responsebtc.json();
        const ethdata = await responseeth.json();
        const bnbdata = await responsebnb.json();

        const btcElement = document.getElementById("rsi-BTCUSDT-4h");
        const ethElement = document.getElementById("rsi-ETHUSDT-4h");
        const bnbElement = document.getElementById("rsi-BNBUSDT-4h");

        btcElement.textContent = btcdata;
        ethElement.textContent = ethdata;
        bnbElement.textContent = bnbdata;

    } catch (err) {
        console.error(err);
    }

    setTimeout(GetRSI4h, 5000);
}

export async function GetRSI1d() {
    try {
        const responsebtc = await fetch("/home/analysis?symbol=btcusdt&interval=1d");
        const responseeth = await fetch("/home/analysis?symbol=ethusdt&interval=1d");
        const responsebnb = await fetch("/home/analysis?symbol=bnbusdt&interval=1d");

        const btcdata = await responsebtc.json();
        const ethdata = await responseeth.json();
        const bnbdata = await responsebnb.json();

        const btcElement = document.getElementById("rsi-BTCUSDT-1d");
        const ethElement = document.getElementById("rsi-ETHUSDT-1d");
        const bnbElement = document.getElementById("rsi-BNBUSDT-1d");

        btcElement.textContent = btcdata;
        ethElement.textContent = ethdata;
        bnbElement.textContent = bnbdata;

    } catch (err) {
        console.error(err);
    }

    setTimeout(GetRSI1d, 5000);
}