import styles from "./SwaggerButton.module.css";

export default function SwaggerButton() {
    if (!import.meta.env.DEV) return null;
    console.log(import.meta.env);
    const swaggerUrl = `${import.meta.env.VITE_API_BASE_URL}/swagger`;

    return (
        <a href={swaggerUrl} target="_blank" className={styles.swaggerButton}>
            Swagger
        </a>
    );
}

/*
// generic option:

type DevLinkProps = {
    label: string;
    href: string;
};

export function DevLink({ label, href }: DevLinkProps) {
    if (!import.meta.env.DEV) return null;

    return (
        <a
            href={href}
            target="_blank"
            style={{
                marginLeft: 12,
                padding: "4px 10px",
                borderRadius: 999,
                background: "#1f2937",
                color: "#fff",
                fontSize: 12,
                textDecoration: "none"
            }}
        >
            {label}
        </a>
    );
}
*/