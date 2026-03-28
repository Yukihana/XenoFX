import Branding from '../Branding/Branding';
import SwaggerButton from '../SwaggerButton/SwaggerButton';
import styles from './Header.module.css';

export default function Header() {
    return (
        <header className={styles.header}>
            <Branding />
            <div className={styles.spacer} />
            <SwaggerButton />
        </header>
    );
}