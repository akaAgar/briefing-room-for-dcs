const renderMainRelease = async () => {
    const result = await fetch("https://api.github.com/repos/akaAgar/briefing-room-for-dcs/releases/latest")
    const release = await result.json()
    console.log(release)
    renderRelease(release, true)
}

const renderBetaRelease = async () => {
    const result = await fetch("https://api.github.com/repos/akaAgar/briefing-room-for-dcs/releases")
    const release = await result.json()
    console.log(release[0])
    renderRelease(release[0])
}

const renderRelease = (release, isLatest = false) => {
    const node = document.createElement("div");

    const title = document.createElement("h3");
    node.appendChild(title)

    const download = document.createElement("a");
    download.textContent = `${isLatest ? "Latest - " : "Beta - "}${release.name}`
    download.setAttribute("href", release.assets[0].browser_download_url)
    title.appendChild(download)

    const converter = new showdown.Converter()
    const changelog = document.createElement("div");
    if (isLatest) {
        changelog.innerHTML = converter.makeHtml(release.body)
    } else {
        const text = document.createElement("p");
        text.innerHTML = release.body.replaceAll("\n", "<br/>").replaceAll(/[0-9a-f]{5,40}/g, '* ')
        changelog.appendChild(text)
    }
    node.appendChild(changelog)
    document.getElementById("releases").appendChild(node)
    document.getElementById("releases").appendChild(document.createElement("hr"))
}

renderBetaRelease()
renderMainRelease()