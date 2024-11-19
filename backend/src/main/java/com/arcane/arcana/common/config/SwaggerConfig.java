package com.arcane.arcana.common.config;

import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import io.swagger.v3.oas.annotations.servers.Server;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import io.swagger.v3.oas.models.Components;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.security.SecurityRequirement;
import io.swagger.v3.oas.models.security.SecurityScheme;

/**
 * Swagger 설정을 구성
 */
@Configuration
@OpenAPIDefinition(
    servers = {
        @Server(url = "https://k11d103.p.ssafy.io", description = "Arcana HTTPS 서버"),
        @Server(url = "http://k11d103.p.ssafy.io", description = "Arcana HTTP 서버"),
        @Server(url = "http://localhost:8080", description = "Local 서버")
    }
)
public class SwaggerConfig {

    @Bean
    public OpenAPI customOpenAPI() {
        return new OpenAPI()
            .components(new Components()
                .addSecuritySchemes("bearer-key",
                    new SecurityScheme()
                        .type(SecurityScheme.Type.HTTP)
                        .scheme("bearer")
                        .bearerFormat("JWT")))
            .info(new Info()
                .title("Arcana API Documentation")
                .version("1.0")
                .description("Arcana 프로젝트의 API 명세서입니다."))
            .addSecurityItem(new SecurityRequirement().addList("bearer-key"));
    }
}
