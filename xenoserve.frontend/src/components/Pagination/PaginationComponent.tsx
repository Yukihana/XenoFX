import styles from './PaginationComponent.module.css';

export default function PaginationComponent() {
    return (
        <div className={styles.pagination}>
            <button className={`${styles.capsule} ${styles.first}`}>{"<<"}</button>

            <div className={`${styles.capsule} ${styles.middle}`}>
                <button className={styles['nav-btn']}>&lt;</button>

                <div className={styles['page-selector']}>
                    <select className={styles['page-dropdown']} defaultValue="1">
                        <option value="1">1</option>
                        <option value="2">2</option>
                        <option value="3">3</option>
                    </select>
                    <input type="text" className={styles['page-input']} defaultValue="1" />
                </div>

                <button className={styles['nav-btn']}>&gt;</button>
            </div>

            <button className={`${styles.capsule} ${styles.last}`}>{">>"}</button>
        </div>
    );
}