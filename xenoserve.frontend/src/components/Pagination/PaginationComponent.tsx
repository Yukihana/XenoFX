export default function PaginationComponent() {
    return (
        <div className="pagination">
            <button>First</button>
            <button>Previous</button>
            {/*Maybe show a list of pages here, conditionally disabling the current page*/}
            {/*Or make current page a text box to allow free navigation*/}
            <div className="current-page">
                <span>0</span> {/*current-page-placeholder*/}
            </div>
            <button>Next</button>
            <button>Last</button>
        </div>
    )
}