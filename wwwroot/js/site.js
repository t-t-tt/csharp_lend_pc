// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.onload = () => {
    document.querySelectorAll(".not-close-dropdown-menu > *").forEach((elm) => {
        elm.addEventListener("click", (e) => {
            e.stopPropagation();
        });
    });

    const tableColumnCheckboxes = document.querySelectorAll(
        ".table_dropdown input[type=checkbox]:not(.check-all)"
    );

    const tableColumnShow = (column, isShowed) => {
        const elms = column
            ? document.querySelectorAll(
                  `.target_table tr > *:nth-child(${column})`
              )
            : document.querySelectorAll(`.target_table tr > *`);
        if (!isShowed) {
            elms.forEach((elm) => {
                elm.style.display = "none";
            });
        } else {
            elms.forEach((elm) => {
                elm.style.display = "table-cell";
            });
        }
    };

    const tableColumnCheckBoxAll = document.querySelector(".table_dropdown .check-all");
    if (tableColumnCheckBoxAll) {
        tableColumnCheckBoxAll.addEventListener("change", (e) => {
            if (!e.target.checked) {
                tableColumnCheckboxes.forEach((elm) => {
                    elm.checked = false;
                    tableColumnShow(elm.value, false);
                });
            } else {
                tableColumnCheckboxes.forEach((elm) => {
                    elm.checked = true;
                    tableColumnShow(elm.value, true);
                });
            }
        });
    }

    tableColumnCheckboxes.forEach((elm) => {
        elm.addEventListener("change", (e) => {
            tableColumnShow(e.target.value, e.target.checked);
            if (!e.target.checked) tableColumnCheckBoxAll.checked = false;
        });
    });

    const tabMenus = document.querySelectorAll("#pills-tab a");
    const tabContents = document.querySelectorAll(
        "#pills-tabContent .tab-pane"
    );
    if (tabMenus) {
        tabMenus.forEach((menu) => {
            menu.addEventListener("click", (e) => {
                const id = e.target.id;
                tabMenus.forEach((elm) => {
                    if (elm.id && elm.id === id) elm.classList.add("active");
                    else elm.classList.remove("active");
                });
                tabContents.forEach((content) => {                    
                    if (
                        content.attributes["aria-labelledby"] &&
                        content.attributes["aria-labelledby"].nodeValue === id
                    )
                    content.classList.add("active", "show");
                    else content.classList.remove("active", "show");
                });
            });
        });
    }
};
